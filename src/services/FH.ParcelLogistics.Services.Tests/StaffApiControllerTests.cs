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

[TestFixture]
public class StaffApiControllerTests
{
    private IMapper CreateAutoMapper(){
        var config = new AutoMapper.MapperConfiguration(cfg => {
            cfg.AddProfile<HelperProfile>();
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
    public void ReportParcelDelivery_ValidTrackingId_Returns200(){
        // arrange
        var validId = GenerateValidTrackingId();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelDelivery(validId))
            .Returns("Successfully reported Hop"); 

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelDelivery(validId) as StatusCodeResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public void ReportParcelDelivery_InvalidTrackingId_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelDelivery(invalidId))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 400)
                        .Build());

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelDelivery(invalidId) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ReportParcelDelivery_ParcelNotFound_Returns404(){
        // arrange
        var validId = GenerateValidTrackingId();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelDelivery(validId))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 404)
                        .Build());

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelDelivery(validId) as ObjectResult;

        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ReportParcelHop_ValidParameters_Returns200(){
        // arrange
        var validId = GenerateValidTrackingId();
        var validCode = GenerateValidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(validId, validCode))
            .Returns("Successfully reported Hop");

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelHop(validId, validCode) as StatusCodeResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public void ReportParcelHop_InvalidTrackingId_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId(); 
        var validCode = GenerateValidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(invalidId, validCode))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 400)
                        .Build());

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelHop(invalidId, validCode) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ReportParcelHop_InvalidCode_Returns400(){
        // arrange
        var validId = GenerateValidTrackingId();
        var invalidCode = GenerateInvalidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(validId, invalidCode))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 400)
                        .Build());

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelHop(validId, invalidCode) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ReportParcelHop_InvalidParameters_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId();
        var invalidCode = GenerateInvalidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(invalidId, invalidCode))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 400)
                        .Build());

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelHop(invalidId, invalidCode) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ReportParcelHop_ParcelNotFound_Returns404(){
        // arrange
        var validId = GenerateValidTrackingId();
        var validCode = GenerateValidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(validId, validCode))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 404)
                        .Build());

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var staffApi = new StaffApiController(mapper, reportingLogic);

        // act
        var result = staffApi.ReportParcelHop(validId, validCode) as ObjectResult;

        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }
}