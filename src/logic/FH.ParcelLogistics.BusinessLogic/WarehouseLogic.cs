using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;


namespace FH.ParcelLogistics.BusinessLogic;

public class WarehouseValidator : AbstractValidator<Warehouse>
{
    public WarehouseValidator(){
        RuleFor(hopType => hopType.HopType).NotNull();
        RuleFor(code => code.Code).NotNull().Matches(@"^[A-Z]{4}\d{1,4}$");
        RuleFor(description => description.Description).NotNull().Matches(@"^[A-Za-zÄÖÜß][A-Za-zÄÖÜß\d -]*$");
        RuleFor(processingDelayMins => processingDelayMins.ProcessingDelayMins).NotNull();
        RuleFor(locationName => locationName.LocationName).NotNull();
        RuleFor(locationCoordinates => locationCoordinates.LocationCoordinates).NotNull();
        RuleFor(level => level.Level).NotNull();
        RuleFor(nextHops => nextHops.NextHops).NotNull();
    }
}

public class WarehouseCodeValidator : AbstractValidator<string>
{
    public WarehouseCodeValidator(){
        RuleFor(code => code).NotNull().Matches(@"^[A-Z]{4}\d{1,4}$");
    }
}

public class WarehouseLogic : IWarehouseLogic
{
    private readonly WarehouseValidator _warehouseValidator = new WarehouseValidator();
    private readonly WarehouseCodeValidator _warehouseCodeValidator = new WarehouseCodeValidator();
    private readonly IHopRepository _hopRepository;

    public WarehouseLogic(){
        _hopRepository = new HopRepository();
    }
    public WarehouseLogic(IHopRepository hopRepository){
        _hopRepository = hopRepository;
    }

    public object ExportWarehouses(){

        // TODO: check for errors
        // if (...) {
        //     return new Error(){
        //         StatusCode = 400,
        //         ErrorMessage = "The operation failed due to an error."
        //     };
        // }

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
        // Validate warehouse
        if(!_warehouseCodeValidator.Validate(code).IsValid){
            return new Error(){
                StatusCode = 400, 
                ErrorMessage = "The operation failed due to an error."
            };
        }

        // TODO: Check if hop exists
        // if(...){
        //     return new Error(){
        //         StatusCode = 404,
        //         ErrorMessage = "No hop with the specified id could be found."
        //     }; 
        // }

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
        // Validate warehouse
        if(!_warehouseValidator.Validate(warehouse).IsValid){
            return new Error(){
                StatusCode = 400, 
                ErrorMessage = "The operation failed due to an error."
            };
        }
        return "Successfully loaded."; 
    }
}
