namespace FH.ParcelLogistics.Services.Tests;

using NUnit.Framework;

using FH.ParcelLogistics.Services.MappingProfiles;
using AutoMapper;
using Moq;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using FizzWare.NBuilder;
using FH.ParcelLogistics.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FH.ParcelLogistics.Services.Controllers;

[TestFixture]
public class WebhookApiControllerTests
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

    private string GenerateValidCode(){
        var codeGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{4}$" });
        return codeGenerator.Generate();
    }

    private string GenerateInvalidCode(){
        var codeGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{5}$" });
        return codeGenerator.Generate();
    }

    [Test]
    public void ListParcelWebhooks_ValidTrackingId_Returns200(){
        // arrange 
        var trackingId = GenerateValidTrackingId();
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.ListWebhooks(trackingId))
            .Returns(Builder<WebhookResponse>.CreateListOfSize(3).Build().ToList());
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        
        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);
        
        // act
        var result = webhookApi.ListParcelWebhooks(trackingId) as ObjectResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public void ListParcelWebhooks_InvalidTrackingId_Returns400(){
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.ListWebhooks(trackingId))
            .Throws<BLValidationException>(); 
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act 
        var result = webhookApi.ListParcelWebhooks(trackingId) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ListParcelWebhooks_ParcelNotFound_Returns404(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.ListWebhooks(trackingId))
            .Throws<BLNotFoundException>();
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.ListParcelWebhooks(trackingId) as ObjectResult;

        // assert 
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void SubscribeParcelWebhook_ValidTrackingId_Returns200(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.Subscribe(trackingId, It.IsAny<string>()))
            .Returns(Builder<WebhookResponse>.CreateNew().Build());
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.SubscribeParcelWebhook(trackingId, "test.com") as ObjectResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public void SubscribeParcelWebhook_InvalidTrackingId_Returns400(){
        // arrange 
        var trackingId = GenerateInvalidTrackingId();
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.Subscribe(trackingId, It.IsAny<string>()))
            .Throws<BLValidationException>();
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.SubscribeParcelWebhook(trackingId, "test.com") as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void SubscribeParcelWebhook_ParcelNotFound_Returns404(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.Subscribe(trackingId, It.IsAny<string>()))
            .Throws<BLNotFoundException>();
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.SubscribeParcelWebhook(trackingId, "test.com") as ObjectResult;

        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void SubscribeParcelWebhook_SubscriptionError_Returns400(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.Subscribe(trackingId, It.IsAny<string>()))
            .Throws<BLException>();
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.SubscribeParcelWebhook(trackingId, "test.com") as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void UnsubscribeParcelWebhook_Returns200(){
        // arrange
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.Unsubscribe(It.IsAny<long>()));
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.UnsubscribeParcelWebhook(1) as StatusCodeResult;

        // assert 
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public void UnsubscribeParcelWebhook_WebhookNotFound_Returns404(){
        // arrange
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.Unsubscribe(It.IsAny<long>()))
            .Throws<BLNotFoundException>();
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.UnsubscribeParcelWebhook(1) as ObjectResult;

        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void UnsubscribeParcelWebhook_UnsubscriptionError_Returns400(){
        // arrange
        var webhookLogicMock = new Mock<IWebhookLogic>();
        webhookLogicMock.Setup(x => x.Unsubscribe(It.IsAny<long>()))
            .Throws<BLException>();
        var webhookLogic = webhookLogicMock.Object;

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;

        var webhookApi = new WebhookApiController(webhookLogic, mapper, logger);

        // act
        var result = webhookApi.UnsubscribeParcelWebhook(1) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }
}