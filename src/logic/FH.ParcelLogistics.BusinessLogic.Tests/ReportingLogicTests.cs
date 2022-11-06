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


public class ReportingLogicTests
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

    private string GenerateValidHopCode()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
        return idGenerator.Generate();
    }

    private string GenerateInvalidHopCode()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{5}\d{1,4}$" });
        return idGenerator.Generate();
    }

    [Test]
    public void TrackingIdValidator_ValidTrackingId_ReturnsTrue()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var validator = new ReportTrackingIDValidator();

        // act
        var result = validator.TestValidate(trackingId);

        // assert
        result?.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void TrackingIdValidator_InvalidTrackingId_ReturnsFalse()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var validator = new ReportTrackingIDValidator();

        // act
        var result = validator.TestValidate(trackingId);

        // assert
        result?.ShouldHaveValidationErrorFor(x => x);
    }

    [Test]
    public void HopCodeValidator_ValidHopCode_ReturnsTrue()
    {
        // arrange
        var hopCode = GenerateValidHopCode();
        var validator = new ReportHopValidator();

        // act
        var result = validator.TestValidate(hopCode);

        // assert
        result?.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void HopCodeValidator_InvalidHopCode_ReturnsFalse()
    {
        // arrange
        var hopCode = GenerateInvalidHopCode();
        var validator = new ReportHopValidator();

        // act
        var result = validator.TestValidate(hopCode);

        // assert
        result?.ShouldHaveValidationErrorFor(x => x);
    }

    [Test]
    public void ReportParcelDelivery_ValidTrackingId_ReturnsSuccess()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DataAccess.Entities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());
        var hopRepositoryMock = new Mock<IHopRepository>();

        var parcelRepository = parcelRepositoryMock.Object;
        var hopRepository = hopRepositoryMock.Object;
        var mapper = CreateAutoMapper();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper);

        // act
        var result = reportingLogic.ReportParcelDelivery(trackingId);

        // assert
        Assert.NotNull(result);
        Assert.AreEqual("Successfully reported hop.", result);
    }

    [Test]
    public void ReportParcelDelivery_InvalidTrackingId_ReturnsError()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DataAccess.Entities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());
        var hopRepositoryMock = new Mock<IHopRepository>();

        var parcelRepository = parcelRepositoryMock.Object;
        var hopRepository = hopRepositoryMock.Object;
        var mapper = CreateAutoMapper();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper);

        // act
        var result = reportingLogic.ReportParcelDelivery(trackingId) as Error;

        // assert
        Assert.NotNull(result);
        Assert.AreEqual(400, result?.StatusCode);
        Assert.AreEqual("The operation failed due to an error.", result?.ErrorMessage);
    }

    [Test]
    public void ReportParcelHop_ValidTrackingIdAndHopCode_ReturnsSuccess()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var hopCode = GenerateValidHopCode();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DataAccess.Entities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());
        var hopRepositoryMock = new Mock<IHopRepository>();

        var parcelRepository = parcelRepositoryMock.Object;
        var hopRepository = hopRepositoryMock.Object;
        var mapper = CreateAutoMapper();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper);

        // act
        var result = reportingLogic.ReportParcelHop(trackingId, hopCode);

        // assert
        Assert.NotNull(result);
        Assert.AreEqual("Successfully reported hop.", result);
    }

    [Test]
    public void ReportParcelHop_InalidTrackingIdAndHopCode_ReturnsError()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var hopCode = GenerateInvalidHopCode();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DataAccess.Entities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());
        var hopRepositoryMock = new Mock<IHopRepository>();

        var parcelRepository = parcelRepositoryMock.Object;
        var hopRepository = hopRepositoryMock.Object;
        var mapper = CreateAutoMapper();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper);

        // act
        var result = reportingLogic.ReportParcelHop(trackingId, hopCode) as Error;

        // assert
        Assert.NotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result?.StatusCode);
        Assert.AreEqual("The operation failed due to an error.", result?.ErrorMessage);
    }
}