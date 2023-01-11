using System.Net;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.Services.MappingProfiles;
using FizzWare.NBuilder;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class TransitionLogicTests
{
    private IMapper CreateAutoMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GeoProfile>();
            cfg.AddProfile<ParcelProfile>();
            cfg.AddProfile<HopProfile>();
        });
        return config.CreateMapper();
    }

    private string GenerateValidTrackingId()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        return idGenerator.Generate();
    }

    private string GenerateInvalidTrackingId()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        return idGenerator.Generate();
    }

    private float GeneratePositiveFloat()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsFloat { Min = 0.1f, Max = 1000f });
        return (float)idGenerator.Generate();
    }

    private string GenerateRandomRegex(string pattern)
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = pattern });
        return idGenerator.Generate();
    }

    private Recipient GenerateValidRecipientObject()
    {
        var recipient = Builder<Recipient>.CreateNew()
            .With(x => x.Name = GenerateRandomRegex(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*"))
            .With(x => x.Country = GenerateRandomRegex(@"Austria|Österreich"))
            .With(x => x.PostalCode = GenerateRandomRegex(@"[A][-]\d{4}"))
            .With(x => x.City = GenerateRandomRegex(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*"))
            .With(x => x.Street = GenerateRandomRegex(@"^[A-Z][a-zäüöß /\d-]*"))
            .Build();
        return recipient;
    }

    private Parcel GenerateValidParcel()
    {
        var parcel = Builder<Parcel>.CreateNew()
            .With(x => x.Weight = GeneratePositiveFloat())
            .With(x => x.Sender = GenerateValidRecipientObject())
            .With(x => x.Recipient = GenerateValidRecipientObject())
            .Build();
        return parcel;
    }

    private Parcel GenerateInvalidParcelwithInvalidWeight()
    {
        var parcel = Builder<Parcel>.CreateNew()
            .With(x => x.Weight = -1.0f)
            .With(x => x.Sender = GenerateValidRecipientObject())
            .With(x => x.Recipient = GenerateValidRecipientObject())

            .Build();
        return parcel;
    }

    [Test]
    public void TransitionTrackingIDValidator_ValidTrackingId_ReturnsTrue()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var validator = new TransitionTrackingIDValidator();

        // act
        var result = validator.TestValidate(trackingId);

        // assert
        result?.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void TransitionTrackingIDValidator_InvalidTrackingId_ReturnsFalse()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var validator = new TransitionTrackingIDValidator();

        // act
        var result = validator.TestValidate(trackingId);

        // assert
        result?.ShouldHaveAnyValidationError();
    }

    [Test]
    public void TransitionValidator_ValidParcel_ReturnsTrue()
    {
        // arrange
        var parcel = GenerateValidParcel();
        var validator = new TransitionValidator();

        // act
        var result = validator.TestValidate(parcel);

        // assert
        result?.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void TransitionValidator_InvalidParcel_ReturnsFalse()
    {
        // arrange
        var parcel = GenerateInvalidParcelwithInvalidWeight();
        var validator = new TransitionValidator();

        // act
        var result = validator.TestValidate(parcel);

        // assert
        result?.ShouldHaveAnyValidationError();
    }

    [Test]
    public void TransitionParcel_ParcelFound_ReturnsConflict()
    {
        // arrange
        var parcel = GenerateValidParcel();
        var trackingId = GenerateValidTrackingId();
        var repositoryMock = new Mock<IParcelRepository>();
        DataAccess.Entities.Parcel outParcel = null;
        repositoryMock.Setup(x => x.TryGetByTrackingId(trackingId, out outParcel)).Returns(true); 
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<TransitionLogic>>();
        var transitionLogic = new TransitionLogic(repository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLConflictException>().And.Message.EqualTo("A parcel with the specified trackingID is already in the system."), () => transitionLogic.TransitionParcel(trackingId, parcel));
    }

    [Test]
    public void TransitionParcel_NoParcelFound_ReturnsParcel()
    {
        // arrange
        var parcel = GenerateValidParcel();
        var trackingId = GenerateValidTrackingId();
        var repositoryMock = new Mock<IParcelRepository>();
        repositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Throws<DALNotFoundException>();
        repositoryMock.Setup(x => x.Submit(It.IsAny<DataAccess.Entities.Parcel>()))
            .Returns(Builder<DataAccess.Entities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .With(x => x.State = DataAccess.Entities.Parcel.ParcelState.Pickup)
                .Build());
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<TransitionLogic>>();
        var transitionLogic = new TransitionLogic(repository, mapper, logger.Object);

        // act
        var result = transitionLogic.TransitionParcel(trackingId, parcel) as BusinessLogic.Entities.Parcel;

        // assert
        Assert.NotNull(result);
        Assert.AreEqual(trackingId, result?.TrackingId);
    }


    [Test]
    public void TransitionParcel_InvalidParcel_InvalidTrackingId_ReturnsError()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var parcel = GenerateInvalidParcelwithInvalidWeight();
        var repositoryMock = new Mock<IParcelRepository>();
        repositoryMock.Setup(x => x.Submit(It.IsAny<DataAccess.Entities.Parcel>()))
            .Returns(Builder<DataAccess.Entities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ITransitionLogic>>();
        var transitionLogic = new TransitionLogic(repository, mapper, logger.Object);

        // act & assert 
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => transitionLogic.TransitionParcel(trackingId, parcel));
    }

    [Test]
    public void TransitionParcel_ValidParcel_SubmitFails_ReturnsError(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcel = GenerateValidParcel();
        var repositoryMock = new Mock<IParcelRepository>();
        repositoryMock.Setup(x => x.Submit(It.IsAny<DataAccess.Entities.Parcel>()))
            .Throws<DALException>();
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ITransitionLogic>>();
        var transitionLogic = new TransitionLogic(repository, mapper, logger.Object);

        // act & assert 
        Assert.Throws(Is.TypeOf<BLException>().And.Message.EqualTo("The operation failed due to an error."), () => transitionLogic.TransitionParcel(trackingId, parcel));
    }
}