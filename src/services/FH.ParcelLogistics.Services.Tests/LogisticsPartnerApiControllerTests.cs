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
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FizzWare.NBuilder;
using FH.ParcelLogistics.Services.DTOs;
using Microsoft.Extensions.Logging;

public class LogisticsPartnerApiControllerTests
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
    public void TransitionParcel_ValidParameters_Returns200(){
        // arrange
        var validId = GenerateValidTrackingId();

        var transitionLogicMock = new Mock<ITransitionLogic>();
        transitionLogicMock.Setup(x => x.TransitionParcel(validId, It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Returns(Builder<BusinessLogic.Entities.Parcel>
                        .CreateNew()
                        .With(x => x.TrackingId = validId)
                        .Build());
        
        var transitionLogic = transitionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var logisticsPartnerApi = new LogisticsPartnerApiController(mapper, transitionLogic, logger);

        // act
        var result = logisticsPartnerApi.TransitionParcel(validId, Builder<DTOs.Parcel>.CreateNew().Build()) as ObjectResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode); 
        Assert.IsInstanceOf<NewParcelInfo>(result?.Value);
        Assert.AreEqual(validId, ((result.Value as NewParcelInfo).TrackingId));
    }

    [Test]
    public void TransitionParcel_InvalidTrackingId_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId();

        var transitionLogicMock = new Mock<ITransitionLogic>();
        transitionLogicMock.Setup(x => x.TransitionParcel(invalidId, It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Throws<BLValidationException>();
        
        var transitionLogic = transitionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var logisticsPartnerApi = new LogisticsPartnerApiController(mapper, transitionLogic, logger);

        // act
        var result = logisticsPartnerApi.TransitionParcel(invalidId, Builder<DTOs.Parcel>.CreateNew().Build()) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode); 
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void TransitionParcel_InvalidParcel_Returns400(){
        // arrange
        var validId = GenerateValidTrackingId();

        var transitionLogicMock = new Mock<ITransitionLogic>();
        transitionLogicMock.Setup(x => x.TransitionParcel(validId, It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Throws<BLValidationException>();
        
        var transitionLogic = transitionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var logisticsPartnerApi = new LogisticsPartnerApiController(mapper, transitionLogic, logger);

        // act
        var result = logisticsPartnerApi.TransitionParcel(validId, Builder<DTOs.Parcel>
                                                                        .CreateNew()
                                                                        .With(x => x.Weight = 0)
                                                                        .Build()) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode); 
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void TransitionParcel_InvalidParameters_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId();

        var transitionLogicMock = new Mock<ITransitionLogic>();
        transitionLogicMock.Setup(x => x.TransitionParcel(invalidId, It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Throws<BLValidationException>();
        
        var transitionLogic = transitionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var logisticsPartnerApi = new LogisticsPartnerApiController(mapper, transitionLogic, logger);

        // act
        var result = logisticsPartnerApi.TransitionParcel(invalidId, Builder<DTOs.Parcel>
                                                                        .CreateNew()
                                                                        .With(x => x.Weight = 0)
                                                                        .Build()) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode); 
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void TransitionParcel_ParcelAlreadyExists_Returns409(){
        // arrange
        var validId = GenerateValidTrackingId();

        var transitionLogicMock = new Mock<ITransitionLogic>();
        transitionLogicMock.Setup(x => x.TransitionParcel(validId, It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Throws<BLConflictException>();
        
        var transitionLogic = transitionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var logisticsPartnerApi = new LogisticsPartnerApiController(mapper, transitionLogic, logger);

        // act
        var result = logisticsPartnerApi.TransitionParcel(validId, Builder<DTOs.Parcel>.CreateNew().Build()) as ObjectResult;

        // assert
        Assert.AreEqual(409, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }
}