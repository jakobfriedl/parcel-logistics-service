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
using Microsoft.Extensions.Logging;

[TestFixture]
public class StaffApiControllerTests
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
    public async Task ReportParcelDelivery_ValidTrackingId_Returns200(){
        // arrange
        var validId = GenerateValidTrackingId();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelDelivery(validId));
        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelDelivery(validId) as StatusCodeResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public async Task ReportParcelDelivery_InvalidTrackingId_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelDelivery(invalidId))
            .Throws<BLValidationException>();

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelDelivery(invalidId) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public async Task ReportParcelDelivery_ParcelNotFound_Returns404(){
        // arrange
        var validId = GenerateValidTrackingId();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelDelivery(validId))
            .Throws<BLNotFoundException>();

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelDelivery(validId) as ObjectResult;

        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public async Task ReportParcelHop_ValidParameters_Returns200(){
        // arrange
        var validId = GenerateValidTrackingId();
        var validCode = GenerateValidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(validId, validCode));

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelHop(validId, validCode) as StatusCodeResult;

        // assert
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public async Task ReportParcelHop_InvalidTrackingId_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId(); 
        var validCode = GenerateValidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(invalidId, validCode))
            .Throws<BLValidationException>();

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelHop(invalidId, validCode) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public async Task ReportParcelHop_InvalidCode_Returns400(){
        // arrange
        var validId = GenerateValidTrackingId();
        var invalidCode = GenerateInvalidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(validId, invalidCode))
            .Throws<BLValidationException>();

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelHop(validId, invalidCode) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public async Task ReportParcelHop_InvalidParameters_Returns400(){
        // arrange
        var invalidId = GenerateInvalidTrackingId();
        var invalidCode = GenerateInvalidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(invalidId, invalidCode))
            .Throws<BLValidationException>();

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelHop(invalidId, invalidCode) as ObjectResult;

        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public async Task ReportParcelHop_ParcelNotFound_Returns404(){
        // arrange
        var validId = GenerateValidTrackingId();
        var validCode = GenerateValidCode();

        var reportingLogicMock = new Mock<IReportingLogic>();
        reportingLogicMock.Setup(x => x.ReportParcelHop(validId, validCode))
            .Throws<BLNotFoundException>();

        var reportingLogic = reportingLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var staffApi = new StaffApiController(mapper, reportingLogic, logger);

        // act
        var result = await staffApi.ReportParcelHop(validId, validCode) as ObjectResult;

        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }
}