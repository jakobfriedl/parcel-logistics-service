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
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class TrackingLogicTests
{
    private IMapper CreateAutoMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<HelperProfile>();
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

    [Test]
    public void TrackingIdValidator_ValidTrackingId_ReturnsTrue()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var validator = new TrackingStateValidator();

        // act
        var result = validator.TestValidate(trackingId);

        // assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test]
    public void TrackingIdValidator_InvalidTrackingId_ReturnsFalse()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var validator = new TrackingStateValidator();

        // act
        var result = validator.TestValidate(trackingId);

        // assert
        result.ShouldHaveValidationErrorFor(x => x);
    }

    // [Test]
    // public void TrackParcel_ValidTrackingId_ReturnsTrackingState()
    // {
    //     // arrange
    //     var trackingId = GenerateValidTrackingId();
    //     var repositoryMock = new Mock<IParcelRepository>();
    //     repositoryMock.Setup(x => x.GetByTrackingId(trackingId))
    //         .Returns(Builder<DataAccess.Entities.Parcel>
    //             .CreateNew()
    //             .With(x => x.TrackingId = trackingId)
    //             .Build());
    //     var repository = repositoryMock.Object;
    //     var mapper = CreateAutoMapper();
    //     var trackingLogic = new TrackingLogic(repository, mapper);

    //     // act
    //     var result = trackingLogic.TrackParcel(trackingId) as ObjectResult;

    //     // assert
    //     Assert.NotNull(result);
    //     Assert.That(result, Is.TypeOf<Parcel>());
    // }
}