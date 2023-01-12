namespace FH.ParcelLogistics.DataAccess.Tests;
using AutoMapper;
using FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FH.ParcelLogistics.ServiceAgents;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using FH.ParcelLogistics.Services.MappingProfiles;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;

public class HopRepositoryTests
{
    private Sql.DbContext _contextMock;
    private IHopRepository _hopRepository;
    private ILogger<IParcelRepository> _loggerMock;
    private IMapper _mapperHopMock;
    private IMapper _mapperGeoMock;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<Sql.DbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var DTruck = Builder<Truck>.CreateNew().Build();
        _contextMock = new Sql.DbContext(options);
        _mapperHopMock = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new HopProfile())));
        _mapperGeoMock = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new GeoProfile())));

        var warehouse = new DataAccess.Entities.Warehouse
        {
            Code = "WH1",
            Description = "Wien 1",
            HopType = "Warehouse",
            Level = 1,
            LocationCoordinates = new NetTopologySuite.Geometries.Point(10, 10),
            LocationName = "Wien 1",
            NextHops = new List<DataAccess.Entities.WarehouseNextHops>{
                new()
                {
                    Hop = new DataAccess.Entities.Truck
                    {
                        Code = "TR1",
                        Description = "Truck 1",
                        HopType = "Truck",
                        LocationCoordinates = new NetTopologySuite.Geometries.Point(10,10),
                        LocationName = "Truck 1 Wien",
                        NumberPlate = "TR1",
                        ProcessingDelayMins = 10,
                        Region = _mapperGeoMock.Map<Geometry>(
                            "{\"type\":\"Feature\",\"geometry\":{\"type\":\"MultiPolygon\",\"coordinates\":[[[[16.3761038,48.2534278],[16.3692824,48.2614423],[16.3682315,48.2613379],[16.369048,48.2571384],[16.3718834,48.2515948],[16.3706547,48.2485833],[16.3672253,48.2446899],[16.3616225,48.2363087],[16.3616607,48.2319017],[16.3634405,48.2287588],[16.3674596,48.2251067],[16.3703691,48.2258219],[16.3721615,48.2278346],[16.3775201,48.2295723],[16.3813071,48.227913],[16.3831208,48.2256264],[16.3846988,48.2260844],[16.388181,48.228166],[16.3867847,48.2303687],[16.398135,48.2363477],[16.389651,48.2440644],[16.3866006,48.2426463],[16.3761038,48.2534278]]]]}}")
                    },
                    TraveltimeMins = 10
                },
                new()
                {
                    Hop = new DataAccess.Entities.Truck
                    {
                        Code = "TR2",
                        Description = "Truck 2",
                        HopType = "Truck",
                        LocationCoordinates = new NetTopologySuite.Geometries.Point(10,10),
                        LocationName = "Truck 2 Wien",
                        NumberPlate = "TR2",
                        ProcessingDelayMins = 10,
                        Region = _mapperGeoMock.Map<Geometry>(
                           "{\"type\":\"Feature\",\"geometry\":{\"type\":\"MultiPolygon\",\"coordinates\":[[[[16.3855063,48.1829183],[16.3809243,48.187927],[16.3688407,48.1838302],[16.3561966,48.1796913],[16.3497357,48.1792195],[16.3416029,48.1764194],[16.3421133,48.1752849],[16.3460717,48.1742029],[16.3471909,48.1727215],[16.3475478,48.1718778],[16.3455778,48.1717031],[16.3451152,48.1698929],[16.3492455,48.1688515],[16.3580045,48.1773158],[16.3627948,48.1764782],[16.3626394,48.1753919],[16.365597,48.1752349],[16.3651894,48.1726012],[16.3757655,48.1707913],[16.372356,48.1645085],[16.3837663,48.1609262],[16.3867273,48.1679921],[16.3895278,48.1663358],[16.3912637,48.1678593],[16.3927152,48.1668226],[16.395348,48.1676099],[16.3948848,48.1690892],[16.3965135,48.1690556],[16.3980274,48.1718422],[16.3967675,48.1723321],[16.3976108,48.1730565],[16.39721,48.1744732],[16.395452,48.1755276],[16.3855063,48.1829183]]]]}}")
                    },
                    TraveltimeMins = 10
                }
            }
        };

        var parcel = new DataAccess.Entities.Parcel
        {
            ParcelId = 1,
            Weight = 2.1f,
            Recipient = new DataAccess.Entities.Recipient
            {
                Name = "Test Name",
                Street = "Test Address",
                City = "Test City",
                Country = "Test Country",
                PostalCode = "A-1100"
            },
            Sender = new DataAccess.Entities.Recipient
            {
                Name = "Test Name",
                Street = "Test Address",
                City = "Test City",
                Country = "Test Country",
                PostalCode = "A-1200"
            },
            TrackingId = "ABCD12345",
            State = Parcel.ParcelState.Pickup,
            VisitedHops = new List<HopArrival>(),
            FutureHops = new List<HopArrival>
            {
                new DataAccess.Entities.HopArrival
                {
                    HopArrivalId = 1,
                    Code = "WH1",
                    Description = "Wien 1",
                    DateTime = DateTime.Now,
                }
            }
        };
        _contextMock.Hops.Add(warehouse);
        _contextMock.SaveChanges();
        _contextMock.Parcels.Add(parcel);
        _contextMock.SaveChanges();

        var loggerMock = new Mock<ILogger<HopRepository>>();
        _hopRepository = new HopRepository(_contextMock, loggerMock.Object);
    }


    [Test]
    public void GetByCode_ReturnsHop(){
        // arrange
        const string code = "TR2";
        // act
        var hop = _hopRepository.GetByCode(code);

        // assert
        Assert.AreEqual("TR2", hop.Code);
        Assert.IsInstanceOf<Hop>(hop);
    }

    [Test]
    public void GetByCode_ThrowsException(){
        // arrange
        const string code = "TR3";

        // act
        var result = _hopRepository.GetByCode(code);

        // assert
        Assert.IsNull(result);
    }        
}