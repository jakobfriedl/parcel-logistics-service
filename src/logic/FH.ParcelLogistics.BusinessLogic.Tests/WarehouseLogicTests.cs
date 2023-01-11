using System.Net;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.Services.MappingProfiles;
using FizzWare.NBuilder;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
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
            cfg.AddProfile<GeoProfile>();
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
    
    private string GenerateValidCode(){
        return GenerateRandomRegex(@"^[A-Z0-9]{4}$");
    }

    private string GenerateInvalidCode(){
        return GenerateRandomRegex(@"^[A-Z0-9]{5}$"); 
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
            .Returns(Builder<DataAccess.Entities.Warehouse>
            .CreateNew()
            .Build());
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<WarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act
        var result = warehouseLogic.ExportWarehouses();

        // assert
        Assert.That(result, Is.TypeOf<Warehouse>());
    }

    [Test]
    public void ExportWarehouses_ThrowsNotFound(){
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.Export())
            .Throws<DALNotFoundException>();
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<WarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLNotFoundException>().And.Message.EqualTo("Root warehouse not found"), () => warehouseLogic.ExportWarehouses());
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
        var logger = new Mock<ILogger<WarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act
        var result = warehouseLogic.GetWarehouse(GenerateRandomRegex(@"^[A-Z]{4}\d{1,4}$"));

        // assert
        Assert.NotNull(result);
        Assert.That(result, Is.TypeOf<Warehouse>());
    }

    [Test]
    public void GetWarehouse_ValidWarehouseCode_ErrorGettingWarehouse()
    {
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.GetByCode(It.IsAny<string>()))
            .Throws<Exception>();
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<WarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLException>().And.Message.EqualTo("Error getting warehouse by code"), () => warehouseLogic.GetWarehouse(GenerateRandomRegex(@"^[A-Z]{4}\d{1,4}$")));
    }

    [Test]
    public void GetWarehouse_InvalidWarehouseCode_ShouldReturnError()
    {
        // arrange
        var code = GenerateInvalidCode();
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.GetByCode(It.IsAny<string>()))
            .Returns(Builder<DataAccess.Entities.Warehouse>
                .CreateNew()
                .Build());
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<WarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => warehouseLogic.GetWarehouse(code));
    }


    [Test]
    public void ImportWarehouses_ValidWarehouse_ShouldReturnSuccess()
    {
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.Import(It.IsAny<DataAccess.Entities.Warehouse>()));
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act & assert
        Assert.DoesNotThrow(() => warehouseLogic.ImportWarehouses(GenerateValidWarehouse()));
    }

    [Test]
    public void ImportWarehouses_InvalidWarehouse_ShouldReturnError()
    {
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.Import(It.IsAny<DataAccess.Entities.Warehouse>()));
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act
        Assert.Throws(Is.TypeOf<BLValidationException>().And.Message.EqualTo("The operation failed due to an error."), () => warehouseLogic.ImportWarehouses(GenerateInvalidWarehouse()));
    }

    [Test]
    public void ImportWarehouses_ErrorImportingWarehouses(){
        // arrange
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(x => x.Import(It.IsAny<DataAccess.Entities.Warehouse>()))
            .Throws<Exception>();
        var repository = repositoryMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<IWarehouseLogic>>();
        var warehouseLogic = new WarehouseLogic(repository, mapper, logger.Object);

        // act & assert
        Assert.Throws(Is.TypeOf<BLException>().And.Message.EqualTo("Error importing warehouse"), () => warehouseLogic.ImportWarehouses(GenerateValidWarehouse()));
    }
}