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

public class StaffApiControllerTests
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
    public void ReportParcelDelivery_ValidTrackingId_Returns200(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var randomTrackingId = idRandomizer.Generate();

        var staffApi = new StaffApiController(_mapper);

        //act
        var result = staffApi.ReportParcelDelivery(randomTrackingId);

        //assert
        Assert.AreEqual(200, (result as StatusCodeResult).StatusCode);
    }

    [Test]
    public void ReportParcelDelivery_InvalidTrackingId_Returns400(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        var randomTrackingId = idRandomizer.Generate();

        var staffApi = new StaffApiController(_mapper);

        //act
        var result = staffApi.ReportParcelDelivery(randomTrackingId);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }

    //TODO - Implement when DB exists
    // [Test]
    // public void ReportParcelDelivery_ParcelNotFound_Returns404(){
    //     //arrange
    //     var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
    //     var randomTrackingId = idRandomizer.Generate();

    //     var staffApi = new StaffApiController(_mapper);

    //     //act
    //     var result = staffApi.ReportParcelDelivery(randomTrackingId);

    //     //assert
    //     Assert.AreEqual(404, (result as ObjectResult).StatusCode);
    // }

    [Test]
    public void ReportParcelHop_ValidParameters_Returns200(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var randomTrackingId = idRandomizer.Generate();
        var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
        var randomCode = codeRandomizer.Generate();

        var staffApi = new StaffApiController(_mapper);

        //act
        var result = staffApi.ReportParcelHop(randomTrackingId, randomCode);

        //assert
        Assert.AreEqual(200, (result as StatusCodeResult).StatusCode);
    }

    [Test]
    public void ReportParcelHop_InvalidTrackingId_Returns400(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        var randomTrackingId = idRandomizer.Generate();
        var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
        var randomCode = codeRandomizer.Generate();

        var staffApi = new StaffApiController(_mapper);

        //act
        var result = staffApi.ReportParcelHop(randomTrackingId, randomCode);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }

    [Test]
    public void ReportParcelHop_InvalidCode_Returns400(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var randomTrackingId = idRandomizer.Generate();
        var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{5}\d{1,4}$" });
        var randomCode = codeRandomizer.Generate();

        var staffApi = new StaffApiController(_mapper);

        //act
        var result = staffApi.ReportParcelHop(randomTrackingId, randomCode);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }

    [Test]
    public void ReportParcelHop_InvalidParameters_Returns400(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{10}$" });
        var randomTrackingId = idRandomizer.Generate();
        var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{5}\d{1,4}$" });
        var randomCode = codeRandomizer.Generate();

        var staffApi = new StaffApiController(_mapper);

        //act
        var result = staffApi.ReportParcelHop(randomTrackingId, randomCode);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }

    // TODO - Implement when DB exists
    // [Test]
    // public void ReportParcelHop_ParcelNotFound_Returns404(){
    //     //arrange
    //     var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
    //     var randomTrackingId = idRandomizer.Generate();
    //     var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
    //     var randomCode = codeRandomizer.Generate();

    //     var staffApi = new StaffApiController(_mapper);

    //     //act
    //     var result = staffApi.ReportParcelHop(randomTrackingId, randomCode);

    //     //assert
    //     Assert.AreEqual(404, (result as ObjectResult).StatusCode);
    // }
}