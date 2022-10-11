using NUnit.Framework;
using AutoMapper;
using FH.ParcelLogistics.Services.MappingProfiles;
using FH.ParcelLogistics.Services.DTOs;

namespace FH.ParcelLogistics.Services.Tests;
public class AutoMapperTests
{
    private MapperConfiguration _config; 
    private IMapper _mapper; 

    [SetUp]
    public void Setup(){
        _config = new MapperConfiguration(cfg => {
            cfg.AddProfile<HelperProfile>();
            cfg.AddProfile<ParcelProfile>();
            cfg.AddProfile<HopProfile>();
        });
        _mapper = _config.CreateMapper();
    }

    [Test]
    public void AutoMapperHasValidConfiguration(){
        _config.AssertConfigurationIsValid();
    }

    [Test]
    public void MapHopDTOToHopEntity(){
        // arrange
        var hopDTO = new DTOs.Hop(){
            Code = "TEST1",
            Description = "Test",
            HopType = "warehouse",
            LocationCoordinates = new DTOs.GeoCoordinate(){
                Lat = 1.0,
                Lon = 1.0
            },
            LocationName = "Test",
            ProcessingDelayMins = 1
        };

        // act
        var hopEntity = _mapper.Map<BusinessLogic.Entities.Hop>(hopDTO);

        // assert
        Assert.AreEqual(hopDTO.Code, hopEntity.Code);
        Assert.AreEqual(hopDTO.Description, hopEntity.Description);
        Assert.AreEqual(hopDTO.HopType, hopEntity.HopType);
        Assert.AreEqual(hopDTO.LocationCoordinates.Lat, hopEntity.LocationCoordinates.Lat);
        Assert.AreEqual(hopDTO.LocationCoordinates.Lon, hopEntity.LocationCoordinates.Lon);
        Assert.AreEqual(hopDTO.LocationName, hopEntity.LocationName);
        Assert.AreEqual(hopDTO.ProcessingDelayMins, hopEntity.ProcessingDelayMins);
    }

    [Test]
    public void MapTruckDTOToTruckEntity(){
        // arrange
        var truckDTO = new DTOs.Truck(){
            Code = "TEST1",
            Description = "Test",
            HopType = "warehouse",
            LocationCoordinates = new DTOs.GeoCoordinate(){
                Lat = 1.0,
                Lon = 1.0
            },
            LocationName = "Test",
            ProcessingDelayMins = 1,
            RegionGeoJson = "Test",
            NumberPlate = "Test",
        };

        // act
        var truckEntity = _mapper.Map<BusinessLogic.Entities.Truck>(truckDTO);

        // assert
        Assert.AreEqual(truckDTO.Code, truckEntity.Code);
        Assert.AreEqual(truckDTO.Description, truckEntity.Description);
        Assert.AreEqual(truckDTO.HopType, truckEntity.HopType);
        Assert.AreEqual(truckDTO.LocationCoordinates.Lat, truckEntity.LocationCoordinates.Lat);
        Assert.AreEqual(truckDTO.LocationCoordinates.Lon, truckEntity.LocationCoordinates.Lon);
        Assert.AreEqual(truckDTO.LocationName, truckEntity.LocationName);
        Assert.AreEqual(truckDTO.ProcessingDelayMins, truckEntity.ProcessingDelayMins);
        Assert.AreEqual(truckDTO.RegionGeoJson, truckEntity.RegionGeoJson);
        Assert.AreEqual(truckDTO.NumberPlate, truckEntity.NumberPlate);
    }

    [Test]
    public void MapWarehouseDTOToWarehouseEntity(){
        // arrange
        var warehouseDTO = new DTOs.Warehouse(){
            Code = "TEST1",
            Description = "Test",
            HopType = "warehouse",
            LocationCoordinates = new DTOs.GeoCoordinate(){
                Lat = 1.0,
                Lon = 1.0
            },
            LocationName = "Test",
            ProcessingDelayMins = 1,
            Level = 1
        };

        // act
        var warehouseEntity = _mapper.Map<BusinessLogic.Entities.Warehouse>(warehouseDTO);

        // assert
        Assert.AreEqual(warehouseDTO.Code, warehouseEntity.Code);
        Assert.AreEqual(warehouseDTO.Description, warehouseEntity.Description);
        Assert.AreEqual(warehouseDTO.HopType, warehouseEntity.HopType);
        Assert.AreEqual(warehouseDTO.LocationCoordinates.Lat, warehouseEntity.LocationCoordinates.Lat);
        Assert.AreEqual(warehouseDTO.LocationCoordinates.Lon, warehouseEntity.LocationCoordinates.Lon);
        Assert.AreEqual(warehouseDTO.LocationName, warehouseEntity.LocationName);
        Assert.AreEqual(warehouseDTO.ProcessingDelayMins, warehouseEntity.ProcessingDelayMins);
        Assert.AreEqual(warehouseDTO.Level, warehouseEntity.Level);
    }

    [Test]
    public void MapParcelDTOsToParcelEntity(){
        // arrange
        var parcelDTO = new DTOs.Parcel(){
            Weight = 1.0f,
            Recipient = new DTOs.Recipient(){
                Name = "Test",
                Street = "Test",
                City = "Test",
                PostalCode = "Test",
                Country = "Test"
            },
            Sender = new DTOs.Recipient(){
                Name = "Test",
                Street = "Test",
                City = "Test",
                PostalCode = "Test",
                Country = "Test"
            }
        }; 

        var newParcelInformationDTO = new DTOs.NewParcelInfo(){
            TrackingId = "Test"
        }; 

        var trackingInformationDTO = new DTOs.TrackingInformation(){
            State = TrackingInformation.ParcelState.Delivered,
            VisitedHops = new List<DTOs.HopArrival>(){},
            FutureHops = new List<DTOs.HopArrival>(){}
        };

        // act
        var parcelEntity = _mapper.Map<BusinessLogic.Entities.Parcel>((parcelDTO, newParcelInformationDTO, trackingInformationDTO));

        // assert
        Assert.AreEqual(parcelDTO.Weight, parcelEntity.Weight);
        Assert.AreEqual(parcelDTO.Recipient.Name, parcelEntity.Recipient.Name);
        Assert.AreEqual(parcelDTO.Sender.Name, parcelEntity.Sender.Name);
        Assert.AreEqual(newParcelInformationDTO.TrackingId, parcelEntity.TrackingId);
        Assert.AreEqual(trackingInformationDTO.State.ToString(), parcelEntity.State.ToString());
        Assert.AreEqual(trackingInformationDTO.VisitedHops.Count, parcelEntity.VisitedHops.Count);
        Assert.AreEqual(trackingInformationDTO.FutureHops.Count, parcelEntity.FutureHops.Count);
    }
}