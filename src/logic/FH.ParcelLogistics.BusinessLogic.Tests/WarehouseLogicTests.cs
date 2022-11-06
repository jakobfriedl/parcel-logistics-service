using System.Net;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.Services.MappingProfiles;
using FizzWare.NBuilder;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace FH.ParcelLogistics.BusinessLogic.Tests;
public class WarehouseLogicTests
{
    private IMapper CreateAutoMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<HelperProfile>();
            cfg.AddProfile<ParcelProfile>();
            cfg.AddProfile<HopProfile>();
        });
        return config.CreateMapper();
    }

    private string GenerateRandomRegex(string pattern)
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = pattern });
        return idGenerator.Generate();
    }

    private int GeneratePositiveInt()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsInteger { Min = 1, Max = 10 });
        return (int)idGenerator.Generate();
    }

    private double GeneratePositiveDouble()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsDouble { Min = 0.1, Max = 1000 });
        return (double)idGenerator.Generate();
    }

    private GeoCoordinate GenerateValidGeoCoordinate()
    {
        var GeoCoordinate = Builder<GeoCoordinate>.CreateNew()
            .With(x => x.Lat = GeneratePositiveDouble())
            .With(x => x.Lon = GeneratePositiveDouble())
            .Build();
        return GeoCoordinate;
    }

    private Hop GenerateValidHop()
    {
        var hop = Builder<Hop>.CreateNew()
            .With(x => x.HopType = GenerateRandomRegex(@"^[A-Z][a-zA-Z]*"))
            .With(x => x.Code = GenerateRandomRegex(@"^[A-Z]{4}\d{1,4}$"))
            .With(x => x.Description = GenerateRandomRegex(@"^[A-Za-zÄÖÜß][A-Za-zÄÖÜß\d -]*$"))
            .With(x => x.ProcessingDelayMins = GeneratePositiveInt())
            .With(x => x.LocationName = GenerateRandomRegex(@"^[A-Z][a-zA-Z]*"))
            .With(x => x.LocationCoordinates = GenerateValidGeoCoordinate())
            .Build();
        return hop;
    }

    private WarehouseNextHops GenerateValidWarehouseNextHop()
    {
        var warehouseNextHops = Builder<WarehouseNextHops>.CreateNew()
            .With(x => x.TraveltimeMins = GeneratePositiveInt())
            .With(x => x.Hop = GenerateValidHop())
            .Build();
        return warehouseNextHops;
    }

    private List<WarehouseNextHops> GenerateValidListOfWarehouseNextHops()
    {
        List<WarehouseNextHops> warehouseNextHops = new List<WarehouseNextHops>(){
            GenerateValidWarehouseNextHop(),
            GenerateValidWarehouseNextHop(),
            GenerateValidWarehouseNextHop()
        };
        return warehouseNextHops;
    }

    private Warehouse GenerateValidWarehouse()
    {
        var warehouse = Builder<Warehouse>.CreateNew()
            .With(x => x.HopType = GenerateRandomRegex(@"[A-Z]*"))
            .With(x => x.Code = GenerateRandomRegex(@"^[A-Z]{4}\d{1,4}$"))
            .With(x => x.Description = GenerateRandomRegex(@"^[A-Za-zÄÖÜß][A-Za-zÄÖÜß\d -]*$"))
            .With(x => x.ProcessingDelayMins = GeneratePositiveInt())
            .With(x => x.LocationName = GenerateRandomRegex(@"^[A-Z]*$"))
            .With(x => x.LocationCoordinates = GenerateValidGeoCoordinate())
            .With(x => x.Level = GeneratePositiveInt())
            .With(x => x.NextHops = GenerateValidListOfWarehouseNextHops())
            .Build();
        return warehouse;
    }

    private Warehouse GenerateInvalidWarehouse()
    {
        var warehouse = Builder<Warehouse>.CreateNew()
            .With(x => x.HopType = GenerateRandomRegex(@"[A-Z]*"))
            .With(x => x.Code = GenerateRandomRegex(@"^[A-Z]*$"))
            .With(x => x.Description = GenerateRandomRegex(@"^[A-Z]*$"))
            .With(x => x.ProcessingDelayMins = GeneratePositiveInt())
            .With(x => x.LocationName = GenerateRandomRegex(@"^[A-Z]*$"))
            .With(x => x.LocationCoordinates = GenerateValidGeoCoordinate())
            .With(x => x.Level = GeneratePositiveInt())
            .With(x => x.NextHops = GenerateValidListOfWarehouseNextHops())
            .Build();
        return warehouse;
    }

    [Test]
    public void WarehouseValidation_ValidWarehouse_ShouldNotHaveValidationError()
    {
        // arrange
        var validator = new WarehouseValidator();
        var warehouse = GenerateValidWarehouse();

        // act
        var result = validator.TestValidate(warehouse);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void WarehouseValidation_InvalidWarehouse_ShouldHaveValidationErrors()
    {
        // arrange
        var validator = new WarehouseValidator();
        var warehouse = GenerateInvalidWarehouse();

        // act
        var result = validator.TestValidate(warehouse);

        // assert
        result.ShouldHaveAnyValidationError();
    }

    [Test]
    public void WarehouseCodeValidator_ValidWarehouseCode_ShouldNotHaveValidationError()
    {
        // arrange
        var validator = new WarehouseCodeValidator();
        var warehouseCode = GenerateRandomRegex(@"^[A-Z]{4}\d{1,4}$");

        // act
        var result = validator.TestValidate(warehouseCode);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void WarehouseCodeValidator_InvalidWarehouseCode_ShouldHaveValidationError()
    {
        // arrange
        var validator = new WarehouseCodeValidator();
        var warehouseCode = GenerateRandomRegex(@"^[A-Z]*$");

        // act
        var result = validator.TestValidate(warehouseCode);

        // assert
        result.ShouldHaveAnyValidationError();
    }

    [Test]
    public void ExportWarehouses_Valid_ShouldReturnValidWarehouse()
    {
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.Export())
            .Returns(true);
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();

        var warehouseLogic = new WarehouseLogic(repository, mapper);

        // act
        var result = warehouseLogic.ExportWarehouses();

        // assert
        Assert.NotNull(result);
        Assert.That(result, Is.TypeOf<Warehouse>());
    }

    [Test]
    public void GetWarehouse_ValidWarehouseCode_ShouldReturnValidHop()
    {
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.GetByCode(It.IsAny<string>()))
            .Returns(Builder<DataAccess.Entities.Warehouse>
                .CreateNew()
                .Build());
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();

        var warehouseLogic = new WarehouseLogic(repository, mapper);

        // act
        var result = warehouseLogic.GetWarehouse(GenerateRandomRegex(@"^[A-Z]{4}\d{1,4}$"));

        // assert
        Assert.NotNull(result);
        Assert.That(result, Is.TypeOf<Hop>());
    }

    [Test]
    public void ImportWarehouses_ValidWarehouse_ShouldReturnSuccess()
    {
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.Import(It.IsAny<DataAccess.Entities.Warehouse>()))
            .Returns(true);
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();

        var warehouseLogic = new WarehouseLogic(repository, mapper);

        // act
        var result = warehouseLogic.ImportWarehouses(GenerateValidWarehouse());

        // assert
        Assert.NotNull(result);
        Assert.AreEqual("Successfully loaded.", result);
    }
}