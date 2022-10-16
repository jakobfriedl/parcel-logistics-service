using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class WarehouseLogicTests
{
    private WarehouseValidator _warehouseValidator;
    private Warehouse _warehouse = new Warehouse(){
        HopType = "Warehouse",
        Code = "ASDF123",
        Description = "Warehouse - 2",
        ProcessingDelayMins = 0,
        LocationName = "Wien",
        LocationCoordinates = new GeoCoordinate(){
            Lat = 48.2081743,
            Lon = 16.3738189
        },
        Level = 1,
        NextHops = new List<WarehouseNextHops>(){
            new WarehouseNextHops(){
                TraveltimeMins = 2,
                Hop = new Hop(){
                    HopType = "Warehouse",
                    Code = "ASDF123",
                    Description = "Warehouse - 2",
                    ProcessingDelayMins = 0,
                    LocationName = "Wien",
                    LocationCoordinates = new GeoCoordinate(){
                        Lat = 48.2081743,
                        Lon = 16.3738189
                    },
                }
            }
        }
    };

    [SetUp]
    public void Setup()
    {
        _warehouseValidator = new WarehouseValidator();
    }

    [Test]
    public void WarehouseValidation()
    {
        // arrange

        // act
        var result = _warehouseValidator.TestValidate(_warehouse);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void WarehouseValidation_Invalid()
    {
        // arrange
        _warehouse.Code = null;

        // act
        var result = _warehouseValidator.TestValidate(_warehouse);

        // assert
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

}