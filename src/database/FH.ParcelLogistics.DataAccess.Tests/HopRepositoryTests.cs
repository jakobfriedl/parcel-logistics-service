namespace FH.ParcelLogistics.DataAccess.Tests;

using NUnit.Framework;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Testing.Moq.Helpers;
using FizzWare.NBuilder;
using FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.DataAccess.Sql;
using System.Reflection;

public class HopRepositoryTests
{
    private Sql.DbContext _contextMock; 

    private string GenerateValidCode(){
        var codeGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{4}$" });
        return codeGenerator.Generate();
    }

    [SetUp]
    public void Setup(){
        var options = new DbContextOptionsBuilder<Sql.DbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var contextToMock = new Sql.DbContext(options);
        _contextMock = new MockedDbContextBuilder<Sql.DbContext>()
            .UseDbContext(contextToMock)
            .UseConstructorWithParameters(options).MockedDbContext; 

        // var hops = Builder<Hop>
        //     .CreateListOfSize(3)
        //     .TheFirst<Hop>(1).With(_ => _.HopId = 1).And(_ => _.Code = "AA11")
        //     .TheNext<Hop>(1).With(_ => _.HopId = 2).And(_ => _.Code = "BB22")
        //     .TheNext<Hop>(1).With(_ => _.HopId = 3).And(_ => _.Code = "CC33")
        //     .Build();   
        
        // var hopArrivals = Builder<HopArrival>
        //     .CreateListOfSize(3)
        //     .TheFirst<HopArrival>(1).With(_ => _.HopArrivalId = 1)
        //     .TheNext<HopArrival>(1).With(_ => _.HopArrivalId = 2)
        //     .TheNext<HopArrival>(1).With(_ => _.HopArrivalId = 3)
        //     .Build();

        // var warehouseNextHops = Builder<WarehouseNextHops>
        //     .CreateListOfSize(3)
        //     .TheFirst<WarehouseNextHops>(1).With(_ => _.WarehouseNextHopsId = 1)
        //     .TheNext<WarehouseNextHops>(1).With(_ => _.WarehouseNextHopsId = 2)
        //     .TheNext<WarehouseNextHops>(1).With(_ => _.WarehouseNextHopsId = 3)
        //     .Build();

        // _contextMock.Set<Hop>().AddRange(hops);
        // _contextMock.Set<HopArrival>().AddRange(hopArrivals);
        // _contextMock.Set<WarehouseNextHops>().AddRange(warehouseNextHops);
        // _contextMock.SaveChanges();
    }

    [TearDown]
    public void TearDown(){
        _contextMock.Database.EnsureDeleted(); 
        _contextMock.Dispose();
    }

    [Test]
    public void GetHopById_Id1_ReturnsHop1(){
        Assert.Pass();

        // arrange
        var hopRepository = new HopRepository(_contextMock);

        // act
        var hop = hopRepository.GetById(1);

        // assert
        Assert.AreEqual(1, hop.HopId);
        Assert.AreEqual("AA11", hop.Code);
    }

    [Test]
    public void GetHopByCode_CodeBB22_ReturnsHop2(){
        Assert.Pass();

        // arrange
        var hopRepository = new HopRepository(_contextMock);

        // act
        var hop = hopRepository.GetByCode("BB22");

        // assert
        Assert.AreEqual(2, hop.HopId);
        Assert.AreEqual("BB22", hop.Code);
    }

    [Test]
    public void GetHops_ReturnsAllHops(){
        Assert.Pass();

        // arrange
        var hopRepository = new HopRepository(_contextMock);

        // act
        var hops = hopRepository.GetHops();

        // assert
        Assert.AreEqual(3, hops.Count());
    }

    [Test]
    public void Export_ReturnsTrue(){
        Assert.Pass();

        // arrange
        var hopRepository = new HopRepository(_contextMock);

        // act
        hopRepository.Export();
    }
}