namespace FH.ParcelLogistics.DataAccess.Tests;

using System.Reflection;
using EntityFrameworkCore.Testing.Moq.Helpers;
using FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

public class HopRepositoryTests
{
    private Sql.DbContext _contextMock; 

    private string GenerateValidCode()
    {
        var codeGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{4}$" });
        return codeGenerator.Generate();
    }

    private string GenerateInvalidCode()
    {
        var codeGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{5}$" });
        return codeGenerator.Generate();
    }

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<Sql.DbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var contextToMock = new Sql.DbContext(options);
        _contextMock = new MockedDbContextBuilder<Sql.DbContext>()
            .UseDbContext(contextToMock)
            .UseConstructorWithParameters(options).MockedDbContext; 

        var hops = Builder<Hop>
            .CreateListOfSize(3)
            .TheFirst<Hop>(1).With(_ => _.HopId = 1).And(_ => _.Code = "AA11")
            .TheNext<Hop>(1).With(_ => _.HopId = 2).And(_ => _.Code = "BB22")
            .TheNext<Hop>(1).With(_ => _.HopId = 3).And(_ => _.Code = "CC33")
            .Build();

        var hopArrivals = Builder<HopArrival>
            .CreateListOfSize(3)
            .TheFirst<HopArrival>(1).With(_ => _.HopArrivalId = 1)
            .TheNext<HopArrival>(1).With(_ => _.HopArrivalId = 2)
            .TheNext<HopArrival>(1).With(_ => _.HopArrivalId = 3)
            .Build();

        var warehouseNextHops = Builder<WarehouseNextHops>
            .CreateListOfSize(3)
            .TheFirst<WarehouseNextHops>(1).With(_ => _.WarehouseNextHopsId = 1)
            .TheNext<WarehouseNextHops>(1).With(_ => _.WarehouseNextHopsId = 2)
            .TheNext<WarehouseNextHops>(1).With(_ => _.WarehouseNextHopsId = 3)
            .Build();

        _contextMock.Set<Hop>().AddRange(hops);
        _contextMock.Set<HopArrival>().AddRange(hopArrivals);
        _contextMock.Set<WarehouseNextHops>().AddRange(warehouseNextHops);
        // _contextMock.SaveChanges();
    }

    [TearDown]
    public void TearDown(){
        _contextMock.Dispose();
    }

    [Test]
    public void GetByCode_CodeBB22_ReturnsHop2(){
        // Arrange
        var loggerMock = new Mock<ILogger<HopRepository>>();
        var hopRepository = new HopRepository(_contextMock, loggerMock.Object);

        // Act
        var hop = hopRepository.GetByCode("BB22");

        // Assert
        Assert.Pass();
    }

    [Test]
    public void Export(){
        // Arrange
        var loggerMock = new Mock<ILogger<HopRepository>>();
        var hopRepository = new HopRepository(_contextMock, loggerMock.Object);

        // Act
        var hops = hopRepository.Export();

        // Assert
        Assert.Pass();
    }
        
}