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
using Microsoft.Extensions.Logging;
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
        var logger = new Mock<ILogger<ReportingLogic>>();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper, logger.Object);

        // act
        reportingLogic.ReportParcelDelivery(trackingId);

        // assert
        Assert.DoesNotThrow(() => reportingLogic.ReportParcelDelivery(trackingId));
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
        var logger = new Mock<ILogger<ReportingLogic>>();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => reportingLogic.ReportParcelDelivery(trackingId));
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
                .With(x => x.FutureHops = Builder<DataAccess.Entities.HopArrival>
                                            .CreateListOfSize(3)
                                            .TheFirst(1).With(x => x.Code = hopCode)
                                            .Build().ToList())
                .With(x => x.VisitedHops = Builder<DataAccess.Entities.HopArrival>.CreateListOfSize(1).Build().ToList())
                .Build());
        var hopRepositoryMock = new Mock<IHopRepository>();
        hopRepositoryMock.Setup(x => x.GetByCode(hopCode))
            .Returns(Builder<DataAccess.Entities.Hop>
                .CreateNew()
                .With(x => x.Code = hopCode)
                .Build()); 

        var parcelRepository = parcelRepositoryMock.Object;
        var hopRepository = hopRepositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ReportingLogic>>();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper, logger.Object);

        // act & assert
        Assert.DoesNotThrow(() => reportingLogic.ReportParcelHop(trackingId, hopCode));
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
        var logger = new Mock<ILogger<IReportingLogic>>();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => reportingLogic.ReportParcelHop(trackingId, hopCode));
    }

    [Test]
    public void ReportParcelHop_ParcelNotFound_ReturnsError()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var hopCode = GenerateValidHopCode();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Throws<DALNotFoundException>();
        var hopRepositoryMock = new Mock<IHopRepository>();
        var parcelRepository = parcelRepositoryMock.Object;
        var hopRepository = hopRepositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ReportingLogic>>();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLNotFoundException>().And.Message.EqualTo("Parcel does not exist with this tracking ID or hop with code not found."), () => reportingLogic.ReportParcelHop(trackingId, hopCode));
    }

    [Test]
    public void ReportParcelDelivery_ParcelNotFound_ReturnsError()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Throws<DALNotFoundException>();
        var hopRepositoryMock = new Mock<IHopRepository>();
        var parcelRepository = parcelRepositoryMock.Object;
        var hopRepository = hopRepositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ReportingLogic>>();
        var reportingLogic = new ReportingLogic(parcelRepository, hopRepository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLNotFoundException>().And.Message.EqualTo("Parcel does not exist with this tracking ID."), () => reportingLogic.ReportParcelDelivery(trackingId));
    }
}