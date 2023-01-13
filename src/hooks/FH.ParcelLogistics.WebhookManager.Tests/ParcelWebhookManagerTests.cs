using System.Linq;
using System.Reflection.Emit;
using AutoMapper;
using DALEntities = FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.Services.MappingProfiles;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Entities;
using Microsoft.Extensions.Logging;
using FH.ParcelLogistics.WebhookManager.Interfaces;
using System.Net.Http;
using System;

namespace FH.ParcelLogistics.WebhookManager.Tests;

public class ParcelWebhookManagerTests
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
    
    [Test]
    public void Notify_CallsWebhooks_DoesNotDelete(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<WebhookResponse>.CreateListOfSize(3).Build().ToList()); 
        webhookRepositoryMock.Setup(x => x.Delete(It.IsAny<long>()));

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<Parcel>.CreateNew()
                .With(x => x.TrackingId = trackingId)
                .And(x => x.State = DALEntities.Parcel.ParcelState.InTransport)
                .Build());

        var mapper = CreateAutoMapper();
        var loggerMock = new Mock<ILogger<IWebhookManager>>();
        var httpClient = new HttpClient(); 

        var parcelWebhookManager = new ParcelWebhookManager(webhookRepositoryMock.Object, parcelRepositoryMock.Object, loggerMock.Object, httpClient, mapper);        

        // act & assert
        Assert.DoesNotThrow(() => parcelWebhookManager.Notify(trackingId));
        webhookRepositoryMock.Verify(x => x.Delete(It.IsAny<long>()), Times.Never);
    }

    [Test]
    public void Notify_CallsWebhooks_DeletesWebhooks(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var webhookRepositoryMock = new Mock<IWebhookRepository>();
        webhookRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<WebhookResponse>.CreateListOfSize(3).Build().ToList());
        webhookRepositoryMock.Setup(x => x.Delete(It.IsAny<long>()));

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(x => x.GetByTrackingId(trackingId))
            .Returns(Builder<Parcel>.CreateNew()
                .With(x => x.TrackingId = trackingId)
                .And(x => x.State = DALEntities.Parcel.ParcelState.Delivered)
                .Build());

        var mapper = CreateAutoMapper();
        var loggerMock = new Mock<ILogger<IWebhookManager>>();
        var httpClient = new HttpClient();

        var parcelWebhookManager = new ParcelWebhookManager(webhookRepositoryMock.Object, parcelRepositoryMock.Object, loggerMock.Object, httpClient, mapper);

        // act & assert
        Assert.DoesNotThrow(() => parcelWebhookManager.Notify(trackingId));
    }
}