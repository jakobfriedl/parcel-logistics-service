namespace FH.ParcelLogistics.Services.Tests;

using System.Reflection.Metadata;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Moq;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.Services.Controllers;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.Services.MappingProfiles;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;

public class RecipientApiControllerTests
{
    private IMapper CreateAutoMapper(){
        var config = new AutoMapper.MapperConfiguration(cfg => {
            cfg.AddProfile<GeoProfile>();
            cfg.AddProfile<HopProfile>();
            cfg.AddProfile<ParcelProfile>();
        });
        return config.CreateMapper();
    }

    private string GenerateValidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        return idGenerator.Generate();
    }

    private string GenerateInvalidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        return idGenerator.Generate();
    }

    [Test]
    public void TrackParcel_ValidTrackingId_Returns200(){
        // arrange
        var validId = GenerateValidTrackingId();

        var trackingLogicMock = new Mock<BusinessLogic.Interfaces.ITrackingLogic>();
        trackingLogicMock.Setup(x => x.TrackParcel(validId))
            .Returns(Builder<BusinessLogic.Entities.Parcel>
                        .CreateNew()
                        .Build());

        var trackingLogic = trackingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var recipientApi = new RecipientApiController(mapper, trackingLogic, logger);

        // act
        var result = recipientApi.TrackParcel(validId) as ObjectResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.TrackingInformation>(result?.Value);
    }

    [Test]
    public void TrackParcel_InvalidTrackingId_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId();

        var trackingLogicMock = new Mock<BusinessLogic.Interfaces.ITrackingLogic>();
        trackingLogicMock.Setup(x => x.TrackParcel(invalidId))
            .Throws<BLValidationException>();
            
        var trackingLogic = trackingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var recipientApi = new RecipientApiController(mapper, trackingLogic, logger);

        // act
        var result = recipientApi.TrackParcel(invalidId) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void TrackParcel_ParcelNotFound_Returns404(){
        // arrange
        var validId = GenerateValidTrackingId();

        var trackingLogicMock = new Mock<BusinessLogic.Interfaces.ITrackingLogic>();
        trackingLogicMock.Setup(x => x.TrackParcel(validId))
            .Throws<BLNotFoundException>();
            
        var trackingLogic = trackingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var recipientApi = new RecipientApiController(mapper, trackingLogic, logger);

        // act
        var result = recipientApi.TrackParcel(validId) as ObjectResult;

        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }
}