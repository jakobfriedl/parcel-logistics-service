using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FluentValidation;


namespace FH.ParcelLogistics.BusinessLogic;

public class WarehouseValidator : AbstractValidator<Warehouse>
{
    public WarehouseValidator()
    {
    }
}

public class WarehouseLogic : IWarehouseLogic
{
    public object ExportWarehouses(){
        return new Warehouse(){
            HopType = "Warehouse",
            Code = "Test",
            Description = "Test",
            ProcessingDelayMins = 1,
            LocationName = "Test",
            LocationCoordinates = new GeoCoordinate(){
                Lat = 1,
                Lon = 1,
            },
            Level = 0,
            NextHops = new List<WarehouseNextHops>(){
                new WarehouseNextHops(){
                    TraveltimeMins = 1,
                    Hop = new Hop(){
                        HopType = "Hop",
                        Code = "Test",
                        Description = "Test",
                        ProcessingDelayMins = 1,
                        LocationName = "Test",
                        LocationCoordinates = new GeoCoordinate(){
                            Lat = 1,
                            Lon = 1,
                        },
                    }
                },
            },
        };
    }

    public object GetWarehouse(string code){
        return new Hop(){
            HopType = "Hop",
            Code = "Test",
            Description = "Test",
            ProcessingDelayMins = 1,
            LocationName = "Test",
            LocationCoordinates = new GeoCoordinate(){
                Lat = 1,
                Lon = 1,
            },
        };
    }

    public object ImportWarehouses(Warehouse warehouse){
        return "test"; 
        // return new Error(){
        //     ErrorMessage = "Not implemented",
        // };
    }
}
