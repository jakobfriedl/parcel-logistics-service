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

public class RecipientApiControllerTests
{
    private IMapper _mapper; 

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
    public void TrackParcel_ValidTrackingId_Returns200(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var randomTrackingId = idRandomizer.Generate();

        var recipientApi = new RecipientApiController(_mapper);

        //act
        var result = recipientApi.TrackParcel(randomTrackingId);

        //assert
        Assert.AreEqual(200, (result as ObjectResult).StatusCode);
    }

    [Test]
    public void TrackParcel_InvalidTrackingId_Returns400(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        var randomTrackingId = idRandomizer.Generate();

        var recipientApi = new RecipientApiController(_mapper);

        //act
        var result = recipientApi.TrackParcel(randomTrackingId);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }

    // TODO - Implement when DB exists
    // [Test]
    // public void TrackParcel_ParcelNotFound_Returns404(){
    //     //arrange
    //     var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
    //     var randomTrackingId = idRandomizer.Generate();

    //     var recipientApi = new RecipientApiController(_mapper);

    //     //act
    //     var result = recipientApi.TrackParcel(randomTrackingId);

    //     //assert
    //     Assert.AreEqual(404, (result as ObjectResult).StatusCode);
    // }
}