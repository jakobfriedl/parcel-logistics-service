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

public class WarehouseManagementApiControllerTests
{
    private IMapper _mapper;

    private readonly DTOs.Warehouse _validWarehouse = new DTOs.Warehouse(){
        HopType = "Warehouse",
        Code = "TEST1",
        Description = "Test Warehouse",
        ProcessingDelayMins = 2,
        LocationName = "Test Location",
        LocationCoordinates = new DTOs.GeoCoordinate(){
            Lat = 48.2081743,
            Lon = 16.3738189
        },
        Level = 1,
        NextHops = new List<DTOs.WarehouseNextHops>(){
            new DTOs.WarehouseNextHops(){
                TraveltimeMins = 2,
                Hop = new DTOs.Hop(){
                    HopType = "Warehouse",
                    Code = "TEST2",
                    Description = "Test Warehouse 2",
                    ProcessingDelayMins = 2,
                    LocationName = "Test Location 2",
                    LocationCoordinates = new DTOs.GeoCoordinate(){
                        Lat = 48.2081743,
                        Lon = 16.3738189
                    },
                }
            }
        }
    };

    private readonly DTOs.Warehouse _invalidWarehouse = new DTOs.Warehouse(){
        HopType = "Warehouse",
        Code = "INVALID",
        Description = "Test Warehouse",
        ProcessingDelayMins = 2,
        LocationName = "Test Location",
        LocationCoordinates = new DTOs.GeoCoordinate(){
            Lat = 48.2081743,
            Lon = 16.3738189
        },
        Level = 1,
        NextHops = new List<DTOs.WarehouseNextHops>(){
            new DTOs.WarehouseNextHops(){
                TraveltimeMins = 2,
                Hop = new DTOs.Hop(){
                    HopType = "Warehouse",
                    Code = "TEST2",
                    Description = "Test Warehouse 2",
                    ProcessingDelayMins = 2,
                    LocationName = "Test Location 2",
                    LocationCoordinates = new DTOs.GeoCoordinate(){
                        Lat = 48.2081743,
                        Lon = 16.3738189
                    },
                }
            }
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
    public void ExportWarehouses_Returns200(){
        //arrange
        var warehouseApi = new WarehouseManagementApiController(_mapper);

        //act
        var result = warehouseApi.ExportWarehouses();
        
        //assert
        Assert.AreEqual(200, (result as ObjectResult).StatusCode);
    }

    //TODO - Mock ExportWarehouses
    // [Test]
    // public void ExportWarehouses_Returns400(){
    //     //arrange
    //     var warehouseApi = new WarehouseManagementApiController(_mapper);

    //     //act
    //     var result = warehouseApi.ExportWarehouses();
        
    //     //assert
    //     Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    // }

    // [Test]
    // public void ExportWarehouses_NoHierarchyLoaded_Returns404(){
    //     //arrange
    //     var warehouseApi = new WarehouseManagementApiController(_mapper);

    //     //act
    //     var result = warehouseApi.ExportWarehouses();
        
    //     //assert
    //     Assert.AreEqual(404, (result as ObjectResult).StatusCode);
    // }

    [Test]
    public void ImportWarehouses_ValidWarehouse_Returns200(){
        //arrange
        var warehouseApi = new WarehouseManagementApiController(_mapper);

        //act
        var result = warehouseApi.ImportWarehouses(_validWarehouse);
        
        //assert
        Assert.AreEqual(200, (result as StatusCodeResult).StatusCode);
    }

    [Test]
    public void ImportWarehouses_InvalidWarehouse_Returns400(){
        //arrange
        var warehouseApi = new WarehouseManagementApiController(_mapper);

        //act
        var result = warehouseApi.ImportWarehouses(_invalidWarehouse);
        
        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }

    [Test]
    public void GetWarehouse_ValidCode_Returns200(){
        //arrange
        var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
        var randomCode = codeRandomizer.Generate();

        var warehouseApi = new WarehouseManagementApiController(_mapper);

        //act
        var result = warehouseApi.GetWarehouse(randomCode);
        
        //assert
        Assert.AreEqual(200, (result as ObjectResult).StatusCode);
    }

    [Test]
    public void GetWarehouse_InvalidCode_Returns400(){
        //arrange
        var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{5}\d{1,4}$" });
        var randomCode = codeRandomizer.Generate();

        var warehouseApi = new WarehouseManagementApiController(_mapper);

        //act
        var result = warehouseApi.GetWarehouse(randomCode);
        
        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }

    // TODO - Mock GetWarehouse 
    // [Test]
    // public void GetWarehouse_HopNotFound_Returns404(){
    //     //arrange
    //     var codeRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
    //     var randomCode = codeRandomizer.Generate();

    //     var warehouseApi = new WarehouseManagementApiController(_mapper);

    //     //act
    //     var result = warehouseApi.GetWarehouse(randomCode);
        
    //     //assert
    //     Assert.AreEqual(404, (result as ObjectResult).StatusCode);
    // }
}