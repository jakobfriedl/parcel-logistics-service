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

public class LogisticsPartnerApiControllerTests
{
    private IMapper _mapper;

    private readonly DTOs.Parcel _validParcel = new DTOs.Parcel(){
        Weight = 1,
        Recipient = new DTOs.Recipient(){
            Name = "John Doe",
            Street = "Street 1",
            City = "City 1",
            PostalCode = "A-1100",
            Country = "Austria"
        },
        Sender = new DTOs.Recipient(){
            Name = "Jane Doe",
            Street = "Street 2",
            City = "City 2",
            PostalCode = "A-1200",
            Country = "Österreich"
        }
    };

    private readonly DTOs.Parcel _invalidParcel = new DTOs.Parcel(){
        Weight = 0,
        Recipient = new DTOs.Recipient(){
            Name = "John Doe",
            Street = "Street 1",
            City = "City 1",
            PostalCode = "B-2323",
            Country = "Austria"
        },
        Sender = new DTOs.Recipient(){
            Name = "Jane Doe",
            Street = "Street 2",
            City = "City 2",
            PostalCode = "A-1200",
            Country = "Germany"
        }
    };

    [SetUp]
    public void Setup(){
        var config = new AutoMapper.MapperConfiguration(cfg => {
            cfg.AddProfile<HelperProfile>();
            cfg.AddProfile<HopProfile>();
            cfg.AddProfile<ParcelProfile>();
        });
        _mapper = config.CreateMapper();
    } 

    [Test]
    public void TransitionParcel_ValidParameters_Returns200(){
        //arrange 
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var randomTrackingId = idRandomizer.Generate();

        var logisticsPartnerApi = new LogisticsPartnerApiController(_mapper);

        //act
        var result = logisticsPartnerApi.TransitionParcel(randomTrackingId, _validParcel);

        //assert
        Assert.AreEqual(200, (result as ObjectResult).StatusCode); 
    }

    [Test]
    public void TransitionParcel_InvalidTrackingId_Returns400(){
        //arrange 
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        var randomTrackingId = idRandomizer.Generate();

        var logisticsPartnerApi = new LogisticsPartnerApiController(_mapper);

        //act
        var result = logisticsPartnerApi.TransitionParcel(randomTrackingId, _validParcel);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode); 
    }

    [Test]
    public void TransitionParcel_InvalidParcel_Returns400(){
        //arrange 
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var randomTrackingId = idRandomizer.Generate();

        var logisticsPartnerApi = new LogisticsPartnerApiController(_mapper);

        //act
        var result = logisticsPartnerApi.TransitionParcel(randomTrackingId, _invalidParcel);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode); 
    }

    [Test]
    public void TransitionParcel_InvalidParameters_Returns400(){
        //arrange 
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        var randomTrackingId = idRandomizer.Generate();

        var logisticsPartnerApi = new LogisticsPartnerApiController(_mapper);

        //act
        var result = logisticsPartnerApi.TransitionParcel(randomTrackingId, _invalidParcel);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode); 
    }

    //TODO - Implement when DB exists
    // [Test]
    // public void TransitionParcel_ParcelAlreadyExists_Returns409(){
    //     //arrange 
    //     var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
    //     var randomTrackingId = idRandomizer.Generate();

    //     var logisticsPartnerApi = new LogisticsPartnerApiController(_mapper);

    //     //act
    //     var result = logisticsPartnerApi.TransitionParcel(randomTrackingId, _validParcel);
    //     result = logisticsPartnerApi.TransitionParcel(randomTrackingId, _validParcel);

    //     //assert
    //     Assert.AreEqual(409, (result as ObjectResult).StatusCode); 
    // }
}