namespace FH.ParcelLogistics.Services.MappingProfiles;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FH.ParcelLogistics.DataAccess.Entities;

[ExcludeFromCodeCoverage]
public class HopProfile : Profile
{
    public HopProfile(){
        CreateMap<DTOs.GeoCoordinate, BusinessLogic.Entities.GeoCoordinate>().ReverseMap();
        CreateMap<DTOs.Hop, BusinessLogic.Entities.Hop>().ReverseMap();
        CreateMap<DTOs.HopArrival, BusinessLogic.Entities.HopArrival>().ReverseMap();
        CreateMap<DTOs.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>().ReverseMap();
        CreateMap<DTOs.WarehouseNextHops, BusinessLogic.Entities.WarehouseNextHops>().ReverseMap();
        CreateMap<DTOs.Truck, BusinessLogic.Entities.Truck>()
            .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>()
            .ReverseMap();
        CreateMap<DTOs.Warehouse, BusinessLogic.Entities.Warehouse>()
            .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>()
            .ReverseMap();

        CreateMap<BusinessLogic.Entities.HopArrival, DataAccess.Entities.HopArrival>().ReverseMap();

        CreateMap<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>().ReverseMap();

        CreateMap<DataAccess.Entities.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>().IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>().ReverseMap();

        CreateMap<DataAccess.Entities.Warehouse, BusinessLogic.Entities.Warehouse>().IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>().ReverseMap();

        CreateMap<DataAccess.Entities.WarehouseNextHops, BusinessLogic.Entities.WarehouseNextHops>().ReverseMap();

        CreateMap<BusinessLogic.Entities.Truck, DataAccess.Entities.Truck>()
            .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom<CustomResolver>());

        CreateMap<DataAccess.Entities.Truck, BusinessLogic.Entities.Truck>()
            .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>()
            .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<CustomResolver>());
    }
}