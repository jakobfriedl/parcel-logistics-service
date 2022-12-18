using System.Net;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using FH.ParcelLogistics.Services.MappingProfiles;
using FizzWare.NBuilder;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class SubmissionLogicTests
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
    public void SubmissionValidator_ValidParcel_ShouldNotHaveValidationError()
    {
        // arrange
        var validator = new SubmissionValidator();
        var parcel = GenerateValidParcel();

        // act
        var result = validator.TestValidate(parcel);

        // assert
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingId);
        result.ShouldNotHaveValidationErrorFor(x => x.Weight);
        result.ShouldNotHaveValidationErrorFor(x => x.Sender);
        result.ShouldNotHaveValidationErrorFor(x => x.Recipient);
    }

    [Test]
    public void SubmissionValidator_WhenInvalidParcelWeight_ShouldHaveValidationError()
    {
        // arrange
        var validator = new SubmissionValidator();
        var parcel = GenerateInvalidParcelwithInvalidWeight();

        // act
        var result = validator.TestValidate(parcel);

        // assert
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }

    [Test]
    public void SubmitParcel_InvalidSubmission_ReturnsError()
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
        var logger = new Mock<ILogger<ISubmissionLogic>>().Object;
                var submissionLogic = new SubmissionLogic(repository, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => submissionLogic.SubmitParcel(parcel));
    }

    [Test]
    public void SubmitParcel_ValidSubmission_ReturnsTrue()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcel = GenerateValidParcel();
        var repositoryMock = new Mock<IParcelRepository>();
        repositoryMock.Setup(x => x.Submit(It.IsAny<DataAccess.Entities.Parcel>()))
            .Returns(Builder<DataAccess.Entities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ISubmissionLogic>>().Object;
        var submissionLogic = new SubmissionLogic(repository, mapper, logger);

        // act
        var result = submissionLogic.SubmitParcel(parcel) as BusinessLogic.Entities.Parcel;

        // assert
        Assert.NotNull(result);
        Assert.AreEqual(trackingId, result.TrackingId);
    }
}