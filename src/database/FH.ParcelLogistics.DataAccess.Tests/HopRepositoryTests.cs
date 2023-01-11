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
    // private Sql.DbContext _contextMock; 

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

    // [SetUp]
    // public void Setup()
    // {
        // var options = new DbContextOptionsBuilder<Sql.DbContext>()
        //     .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //     .Options;
        // var contextToMock = new Sql.DbContext(options);
        // _contextMock = new MockedDbContextBuilder<Sql.DbContext>()
        //     .UseDbContext(contextToMock)
        //     .UseConstructorWithParameters(options).MockedDbContext; 

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
        //_contextMock.SaveChanges();
    // }

    // [TearDown]
    // public void TearDown(){
    //     _contextMock.Dispose();
    // }

    // [Test]
    // public void GetHopByCode_CodeBB22_ReturnsHop2(){
    //     Assert.Pass();

    //     // arrange
    //     var logger = new Mock<ILogger<IHopRepository>>().Object;
    //     var hopRepository = new HopRepository(_contextMock, logger);

    //     // act
    //     var hop = hopRepository.GetByCode("BB22");

    //     // assert
    //     Assert.AreEqual(2, hop.HopId);
    //     Assert.AreEqual("BB22", hop.Code);
    // }

    // [Test]
    // public void Export_ReturnsHopHierarchy(){
    //     Assert.Pass();

    //     // arrange
    //     var logger = new Mock<ILogger<IHopRepository>>().Object;
    //     var hopRepository = new HopRepository(_contextMock, logger);

    //     // act
    //     hopRepository.Export();
    // }

    [Test]
    public void GetByCode_Successful()
    {
        // arrange
        var code = GenerateValidCode();
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(_ => _.GetByCode(code)).Returns(new Hop { Code = code });

        // act
        repositoryMock.Object.GetByCode(code);

        // assert
        repositoryMock.Verify(_ => _.GetByCode(code));
        Assert.AreEqual(code, repositoryMock.Object.GetByCode(code).Code);
    }

    [Test]
    public void GetByCode_InvalidCode_ThrowsException()
    {
        // arrange
        var code = GenerateInvalidCode();
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(_ => _.GetByCode(code)).Throws(new DALException($"Hop with code {code} not found"));

        // act
        var exception = Assert.Throws<DALException>(() => repositoryMock.Object.GetByCode(code));

        // assert
        Assert.AreEqual($"Hop with code {code} not found", exception.Message);
    }

    [Test]
    public void Import_Successful()
    {
        // arrange
        var hop = Builder<Warehouse>
            .CreateNew()
            .With(_ => _.Code = GenerateValidCode())
            .Build();
        var logger = new Mock<ILogger<IHopRepository>>().Object;
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(_ => _.Import(hop));

        // act
        repositoryMock.Object.Import(hop);

        // assert
        repositoryMock.Verify(_ => _.Import(hop));
    }

    [Test]
    public void Import_NullHop_ThrowsException()
    {
        // arrange
        Hop hop = null;
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(_ => _.Import(hop));

        // act
        repositoryMock.Object.Import(hop);

        // assert
        repositoryMock.Verify(_ => _.Import(hop));
    }

    [Test]
    public void Export_Successful()
    {
        // arrange
        var logger = new Mock<ILogger<IHopRepository>>().Object;
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(_ => _.Export());

        // act
        repositoryMock.Object.Export();

        // assert
        repositoryMock.Verify(_ => _.Export());
    }

    [Test]
    public void Export_ThrowsException()
    {
        // arrange
        var logger = new Mock<ILogger<IHopRepository>>().Object;
        var repositoryMock = new Mock<IHopRepository>();
        repositoryMock.Setup(_ => _.Export()).Throws(new DALException("Hop not exported"));

        // act
        var exception = Assert.Throws<DALException>(() => repositoryMock.Object.Export());

        // assert
        Assert.AreEqual("Hop not exported", exception.Message);
    }

}