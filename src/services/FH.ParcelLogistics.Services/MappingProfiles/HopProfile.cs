namespace FH.ParcelLogistics.Services.MappingProfiles;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;

[ExcludeFromCodeCoverage]
public class HopProfile : Profile
{
    public HopProfile(){
        CreateMap<DTOs.GeoCoordinate, BusinessLogic.Entities.GeoCoordinate>().ReverseMap();
        CreateMap<DTOs.Hop, BusinessLogic.Entities.Hop>().ReverseMap();
        CreateMap<DTOs.HopArrival, BusinessLogic.Entities.HopArrival>().ReverseMap();
        CreateMap<DTOs.Recipient, BusinessLogic.Entities.Recipient>().ReverseMap();
        CreateMap<DTOs.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>().ReverseMap();
        CreateMap<DTOs.WarehouseNextHops, BusinessLogic.Entities.WarehouseNextHops>().ReverseMap();
        CreateMap<DTOs.Truck, BusinessLogic.Entities.Truck>()
            .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>()
            .ReverseMap();
        CreateMap<DTOs.Warehouse, BusinessLogic.Entities.Warehouse>()
            .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>()
            .ReverseMap();
    }
}
