namespace FH.ParcelLogistics.DataAccess.Tests;

using BingMapsRESTToolkit;
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

    private string GenerateInvalidTrackingId()
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{8}$" });
        return idGenerator.Generate();
    }

    private string GenerateValidCode()
    {
        var codeGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{4}$" });
        return codeGenerator.Generate();
    }

    private Hop GenerateHop()
    {
        var hop = new Hop();
        hop.HopType = "Warehouse";
        hop.Code = GenerateValidCode();
        hop.Description = "Test Hop";
        hop.ProcessingDelayMins = 10;
        hop.LocationName = "Test Location";

        return hop;
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
            .TheNext<Parcel>(1).With(_ => _.TrackingId = "305P2O7EC").And(_ => _.ParcelId = 3).And(_ => _.State = Parcel.ParcelState.Pickup)
            .Build();

        _contextMock.Set<Parcel>().AddRange(parcels);
        _contextMock.SaveChanges();
    }

    [TearDown]
    public void TearDown(){
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
    public void TryGetByTrackingId_305P2O7EC_ReturnsTrue(){
        // arrange
        var logger = new Mock<ILogger<IParcelRepository>>().Object;
        var agent = new Mock<IGeoEncodingAgent>().Object;
        var parcelRepository = new ParcelRepository(_contextMock, logger, agent);

        // act
        var result = parcelRepository.TryGetByTrackingId("305P2O7EC", out var parcel);

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual(3, parcel.ParcelId);
        Assert.AreEqual("305P2O7EC", parcel.TrackingId);
    }

    [Test]
    public void TryGetByTrackingId_405P2O7EC_ReturnsFalse(){
        // arrange
        var logger = new Mock<ILogger<IParcelRepository>>().Object;
        var agent = new Mock<IGeoEncodingAgent>().Object;
        var parcelRepository = new ParcelRepository(_contextMock, logger, agent);

        // act
        var result = parcelRepository.TryGetByTrackingId("405P2O7ED", out var parcel);

        // assert
        Assert.IsFalse(result);
        Assert.IsNull(parcel);
    }

    [Test]
    public void Update_ReturnsUpdatedParcel(){
        // arrange
        var logger = new Mock<ILogger<IParcelRepository>>().Object;
        var agent = new Mock<IGeoEncodingAgent>().Object;
        var parcelRepository = new ParcelRepository(_contextMock, logger, agent);
        var parcel = Builder<Parcel>
            .CreateNew()
            .With(_ => _.ParcelId = 3)
            .With(_ => _.TrackingId = "305P2O7EC")
            .With(_ => _.State = Parcel.ParcelState.Delivered)
            .Build();

        // act
        var result = parcelRepository.Update(parcel);

        // assert
        Assert.AreEqual(parcel, result);
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

    [Test]
    public void GetByTrackingId_Successfull()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.GetByTrackingId(trackingId)).Returns(new Parcel
        {
            TrackingId = trackingId
        });

        // act
        var parcel = parcelRepositoryMock.Object.GetByTrackingId(trackingId);

        // assert
        Assert.AreEqual(trackingId, parcel.TrackingId);
    }

    [Test]
    public void GetByTrackingId_ThrowsException()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.GetByTrackingId(trackingId)).Throws(new Exception());

        // act
        var exception = Assert.Throws<Exception>(() => parcelRepositoryMock.Object.GetByTrackingId(trackingId));

        // assert
        Assert.IsNotNull(exception);
    }

    [Test]
    public void TryGetByTrackingId_Successfull()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.TryGetByTrackingId(trackingId, out It.Ref<Parcel>.IsAny)).Returns(true);

        // act
        var result = parcelRepositoryMock.Object.TryGetByTrackingId(trackingId, out var parcel);

        // assert
        Assert.IsTrue(result);
    }

    [Test]
    public void TryGetByTrackingId_ThrowsException()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.TryGetByTrackingId(trackingId, out It.Ref<Parcel>.IsAny)).Throws(new Exception());

        // act
        var exception = Assert.Throws<Exception>(() => parcelRepositoryMock.Object.TryGetByTrackingId(trackingId, out var parcel));

        // assert
        Assert.IsNotNull(exception);
    }

    [Test]
    public void Submit_Successfull()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.Submit(It.IsAny<Parcel>())).Returns(new Parcel
        {
            TrackingId = trackingId
        });

        // act
        var parcel = parcelRepositoryMock.Object.Submit(new Parcel());

        // assert
        Assert.AreEqual(trackingId, parcel.TrackingId);
    }

    [Test]
    public void Submit_ThrowsException()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.Submit(It.IsAny<Parcel>())).Throws(new Exception());

        // act
        var exception = Assert.Throws<Exception>(() => parcelRepositoryMock.Object.Submit(new Parcel()));

        // assert
        Assert.IsNotNull(exception);
    }

    [Test]
    public void Update_Successfull()
    {
        // arrange
        var trackingId = GenerateValidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.Update(It.IsAny<Parcel>())).Returns(new Parcel
        {
            TrackingId = trackingId
        });

        // act
        var parcel = parcelRepositoryMock.Object.Update(new Parcel());

        // assert
        Assert.AreEqual(trackingId, parcel.TrackingId);
    }

    [Test]
    public void Update_ThrowsException()
    {
        // arrange
        var trackingId = GenerateInvalidTrackingId();
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.Update(It.IsAny<Parcel>())).Throws(new Exception());

        // act
        var exception = Assert.Throws<Exception>(() => parcelRepositoryMock.Object.Update(new Parcel()));

        // assert
        Assert.IsNotNull(exception);
    }

    [Test]
    public void PredictRoute_Successfull_SameHop()
    {
        // arrange
        var hopA = new Hop
        {
            HopId = 1,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien01",
            ProcessingDelayMins = 10,
            LocationName = "Wien01",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(1, 1)
        };

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.PredictRoute(hopA, hopA)).Returns(new List<HopArrival>
        {
            new HopArrival
            {
                HopArrivalId = 1,
                Code = GenerateValidCode(),
                Description = "Wien01",
                DateTime = DateTime.Now   
            }
        });

        // act
        var result = parcelRepositoryMock.Object.PredictRoute(hopA, hopA);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [Test]
    public void PredictRoute_Successfull_DifferentHops()
    {
        // arrange
        var hopA = new Hop
        {
            HopId = 1,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien01",
            ProcessingDelayMins = 10,
            LocationName = "Wien01",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(1, 1)
        };

        var hopB = new Hop
        {
            HopId = 2,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien02",
            ProcessingDelayMins = 10,
            LocationName = "Wien02",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(2, 2)
        };

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.PredictRoute(hopA, hopB)).Returns(new List<HopArrival>
        {
            new HopArrival
            {
                HopArrivalId = 1,
                Code = GenerateValidCode(),
                Description = "Wien01",
                DateTime = DateTime.Now   
            },
            new HopArrival
            {
                HopArrivalId = 2,
                Code = GenerateValidCode(),
                Description = "Wien02",
                DateTime = DateTime.Now   
            }
        });

        // act
        var result = parcelRepositoryMock.Object.PredictRoute(hopA, hopB);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [Test]
    public void PredictRoute_ThrowsException()
    {
        // arrange
        var hopA = new Hop
        {
            HopId = 1,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien01",
            ProcessingDelayMins = 10,
            LocationName = "Wien01",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(1, 1)
        };

        var hopB = new Hop
        {
            HopId = 2,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien02",
            ProcessingDelayMins = 10,
            LocationName = "Wien02",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(2, 2)
        };

        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.PredictRoute(hopA, hopB)).Throws(new Exception());

        // act
        var exception = Assert.Throws<Exception>(() => parcelRepositoryMock.Object.PredictRoute(hopA, hopB));

        // assert
        Assert.IsNotNull(exception);
    }

    [Test]
    public void Parent_Successfull()
    {
        // arrange
        var Hop = new Hop
        {
            HopId = 1,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien01",
            ProcessingDelayMins = 10,
            LocationName = "Wien01",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(1, 1)
        };
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.Parent(Hop)).Returns(new Hop
        {
            HopId = 1,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien01",
            ProcessingDelayMins = 10,
            LocationName = "Wien01",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(1, 1)
        });

        // act
        var result = parcelRepositoryMock.Object.Parent(Hop);

        // assert
        Assert.IsNotNull(result);
    }

    [Test]
    public void Parent_ThrowsException()
    {
        // arrange
        var Hop = new Hop
        {
            HopId = 1,
            HopType = "In Delivery",
            Code = GenerateValidCode(),
            Description = "Wien01",
            ProcessingDelayMins = 10,
            LocationName = "Wien01",
            LocationCoordinates = new NetTopologySuite.Geometries.Point(1, 1)
        };
        var parcelRepositoryMock = new Mock<IParcelRepository>();
        parcelRepositoryMock.Setup(_ => _.Parent(Hop)).Throws(new Exception());

        // act
        var exception = Assert.Throws<Exception>(() => parcelRepositoryMock.Object.Parent(Hop));

        // assert
        Assert.IsNotNull(exception);
    }
}