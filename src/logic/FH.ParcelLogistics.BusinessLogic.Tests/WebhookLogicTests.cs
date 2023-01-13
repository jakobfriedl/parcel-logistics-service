using AutoMapper;
using BLEntities = FH.ParcelLogistics.BusinessLogic.Entities;
using DALEntities = FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.Services.MappingProfiles;
using FizzWare.NBuilder;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using FH.ParcelLogistics.DataAccess.Entities;

namespace FH.ParcelLogistics.BusinessLogic.Tests;
public class WebhookLogicTests
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

    private string GenerateValidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        return idGenerator.Generate();
    }

    private string GenerateInvalidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        return idGenerator.Generate();
    }

    [Test]
    public void TrackingIDValidator_ValidTrackingId_ReturnsTrue()
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
    public void TrackingIDValidator_InvalidTrackingId_ReturnsFalse()
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
    public void ListWebhooks_ValidTrackingsId_ReturnsWebhooks()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DALEntities.WebhookResponse>
                .CreateListOfSize(3)
                .All().With(x => x.TrackingId = trackingId)
                .Build().ToList());

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DALEntities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act
        var result = webhookLogic.ListWebhooks(trackingId);

        // assert
        Assert.AreEqual(3, result.Count());
        Assert.AreEqual(trackingId, result.First().TrackingId);
        Assert.DoesNotThrow(() => webhookLogic.ListWebhooks(trackingId));
    }

    [Test]
    public void ListWebhooks_InvalidTrackingId_ReturnsError(){
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        var parcelRepositoryMock = new Mock<IParcelRepository>();

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => webhookLogic.ListWebhooks(trackingId));
    }

    [Test]
    public void ListWebhooks_ParcelNotFound_ReturnsError(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Throws<DALNotFoundException>();
        
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLNotFoundException>().And.Message.EqualTo("No parcel found with that tracking ID."), () => webhookLogic.ListWebhooks(trackingId));
    }

    [Test]
    public void Subscribe_ValidTrackingId_ReturnsWebhook(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhook = Builder<DALEntities.WebhookResponse>
            .CreateNew()
            .With(x => x.TrackingId = trackingId)
            .Build();

        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.Create(It.IsAny<DALEntities.WebhookResponse>()))
            .Returns(webhook); 

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DALEntities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act
        var result = webhookLogic.Subscribe(trackingId, webhook.Url);

        // assert
        Assert.AreEqual(trackingId, result.TrackingId);
    }

    [Test]
    public void Subscribe_InvalidTrackingId_ReturnsError(){
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var webhook = Builder<DALEntities.WebhookResponse>
            .CreateNew()
            .With(x => x.TrackingId = trackingId)
            .Build();
        
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.Create(It.IsAny<DALEntities.WebhookResponse>()));

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId));

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => webhookLogic.Subscribe(trackingId, webhook.Url));
    }

    [Test]
    public void Subscribe_ParcelNotFound_ReturnsError(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhook = Builder<DALEntities.WebhookResponse>
            .CreateNew()
            .With(x => x.TrackingId = trackingId)
            .Build();
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.Create(It.IsAny<DALEntities.WebhookResponse>()));

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Throws<DALNotFoundException>();

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLNotFoundException>().And.Message.EqualTo("No parcel found with that tracking ID."), () => webhookLogic.Subscribe(trackingId, webhook.Url));
    }

    [Test]
    public void Subscribe_CreateFailed_ReturnsError(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhook = Builder<DALEntities.WebhookResponse>
            .CreateNew()
            .With(x => x.TrackingId = trackingId)
            .Build();

        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.Create(It.IsAny<DALEntities.WebhookResponse>()))
            .Throws<DALException>();
        
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<DALEntities.Parcel>
                .CreateNew()
                .With(x => x.TrackingId = trackingId)
                .Build());
            
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLException>().And.Message.EqualTo("The operation failed due to an error."), () => webhookLogic.Subscribe(trackingId, webhook.Url));
    }

    [Test]
    public void Unsubscribe_IdFound(){
        // arrange
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.Delete(It.IsAny<long>()));

        var parcelRepositoryMock = new Mock<IParcelRepository>();

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.DoesNotThrow(() => webhookLogic.Unsubscribe(1));
    }

    [Test]
    public void Unsubscribe_IdNotFound(){
        // arrange
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.Delete(It.IsAny<long>()))
            .Throws<DALNotFoundException>();

        var parcelRepositoryMock = new Mock<IParcelRepository>();

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLNotFoundException>().And.Message.EqualTo("Subscription does not exist."), () => webhookLogic.Unsubscribe(1));
    }

    [Test]
    public void Unsubscribe_DeleteFailed(){
        // arrange
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.Delete(It.IsAny<long>()))
            .Throws<DALException>();

        var parcelRepositoryMock = new Mock<IParcelRepository>();

        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWebhookLogic>>().Object;

        var webhookLogic = new WebhookLogic(webhookRepositoryMock.Object, parcelRepositoryMock.Object, mapper, logger);

        // act & assert
        Assert.Throws(Is.TypeOf<BLException>().And.Message.EqualTo("The operation failed due to an error."), () => webhookLogic.Unsubscribe(1));
    }
}