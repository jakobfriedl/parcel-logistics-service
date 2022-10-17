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
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Host;
using Microsoft.AspNetCore.Mvc;
using FizzWare.NBuilder;
using FH.ParcelLogistics.BusinessLogic.Interfaces;

public class WarehouseManagementApiControllerTests
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
    public void ExportWarehouses_Returns200(){
        // arrange
        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.ExportWarehouses())
            .Returns(Builder<BusinessLogic.Entities.Warehouse>.CreateNew().Build());

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.ExportWarehouses() as ObjectResult;
        
        // assert
        Assert.AreEqual(200, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Warehouse>(result?.Value);
    }

    [Test]
    public void ExportWarehouses_Returns400(){
        // arrange
        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.ExportWarehouses())
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 400)
                        .Build());

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.ExportWarehouses() as ObjectResult;
        
        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ExportWarehouses_NoHierarchyLoaded_Returns404(){
        // arrange
        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.ExportWarehouses())
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 404)
                        .Build());

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.ExportWarehouses() as ObjectResult;
        
        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void ImportWarehouses_ValidWarehouse_Returns200(){
        // arrange
        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.ImportWarehouses(It.IsAny<BusinessLogic.Entities.Warehouse>()))
            .Returns("Successfully loaded.");

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.ImportWarehouses(Builder<DTOs.Warehouse>
                                                        .CreateNew()
                                                        .With(x => x.Code = GenerateValidCode())
                                                        .Build()) as StatusCodeResult;
        
        // assert
        Assert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public void ImportWarehouses_InvalidWarehouse_Returns400(){
        // arrange
        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.ImportWarehouses(It.IsAny<BusinessLogic.Entities.Warehouse>()))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 400)
                        .Build());

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.ImportWarehouses(Builder<DTOs.Warehouse>
                                                        .CreateNew()
                                                        .With(x => x.Code = GenerateInvalidCode())
                                                        .Build()) as ObjectResult;
        
        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void GetWarehouse_ValidCode_Returns200(){
        // arrange
        var validCode = GenerateValidCode();

        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.GetWarehouse(validCode))
            .Returns(Builder<BusinessLogic.Entities.Warehouse>.CreateNew().Build());

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.GetWarehouse(validCode) as ObjectResult;
        
        // assert
        Assert.AreEqual(200, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Warehouse>(result?.Value);
    }

    [Test]
    public void GetWarehouse_InvalidCode_Returns400(){
        // arrange
        var invalidCode = GenerateInvalidCode();

        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.GetWarehouse(invalidCode))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 400)
                        .Build());

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.GetWarehouse(invalidCode) as ObjectResult;
        
        // assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void GetWarehouse_HopNotFound_Returns404(){
        // arrange
        var validCode = GenerateValidCode();

        var warehouseLogicMock = new Mock<IWarehouseLogic>();
        warehouseLogicMock.Setup(x => x.GetWarehouse(validCode))
            .Returns(Builder<BusinessLogic.Entities.Error>
                        .CreateNew()
                        .With(x => x.StatusCode = 404)
                        .Build());

        var warehouseLogic = warehouseLogicMock.Object;
        var mapper = CreateAutoMapper();
        var warehouseApi = new WarehouseManagementApiController(mapper, warehouseLogic);

        // act
        var result = warehouseApi.GetWarehouse(validCode) as ObjectResult;
        
        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }
}