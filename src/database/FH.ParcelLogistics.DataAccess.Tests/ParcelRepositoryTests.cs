namespace FH.ParcelLogistics.DataAccess.Tests;

using System.Diagnostics;
using System.Reflection.Emit;
using EntityFrameworkCore.Testing.Moq;
using EntityFrameworkCore.Testing.Moq.Helpers;
using FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

public class ParcelRepositoryTests
{
    private Sql.DbContext _contextMock; 

    private string GenerateValidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        return idGenerator.Generate();
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

        var parcels = Builder<Parcel>
            .CreateListOfSize(3)
            .TheFirst<Parcel>(1).With(_ => _.TrackingId = "ABCDEFGHI").And(_ => _.ParcelId = 1)
            .TheNext<Parcel>(1).With(_ => _.TrackingId = "BCDEFGHIJ").And(_ => _.ParcelId = 2)
            .TheNext<Parcel>(1).With(_ => _.TrackingId = "305P2O7EC").And(_ => _.ParcelId = 3)
            .Build();

        _contextMock.Set<Parcel>().AddRange(parcels);
        _contextMock.SaveChanges();
    }

    [TearDown]
    public void TearDown(){
        _contextMock.Database.EnsureDeleted(); 
        _contextMock.Dispose();
    }

    [Test]
    public void GetByTrackingId_305P2O7EC_ReturnsParcel3(){
        // arrange
        var logger = new Mock<ILogger<IParcelRepository>>().Object;
        var agent = new Mock<IGeoEncodingAgent>().Object;
        var parcelRepository = new ParcelRepository(_contextMock, logger, agent);

        // act
        var parcel = parcelRepository.GetByTrackingId("305P2O7EC");

        // assert
        Assert.AreEqual(3, parcel.ParcelId);
        Assert.AreEqual("305P2O7EC", parcel.TrackingId);
    }

    [Test]
    public void Submit_ReturnsParcel(){
        // arrange
        var trackingId = GenerateValidTrackingId();
        var logger = new Mock<ILogger<IParcelRepository>>().Object;
        var agent = new Mock<IGeoEncodingAgent>().Object;
        var parcelRepository = new ParcelRepository(_contextMock, logger, agent);
        var parcel = Builder<Parcel>
            .CreateNew()
            .With(_ => _.ParcelId = 4)
            .With(_ => _.TrackingId = trackingId)
            .Build();

        // act
        var result = parcelRepository.Submit(parcel);

        // assert
        Assert.AreEqual(4, result.ParcelId);
        Assert.AreEqual(trackingId, result.TrackingId);
    }
}