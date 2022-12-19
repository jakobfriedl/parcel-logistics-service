namespace FH.ParcelLogistics.Services.MappingProfiles;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FH.ParcelLogistics.DataAccess.Entities;
using NetTopologySuite.Geometries;

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

        // Transferwarehouse
        CreateMap<DataAccess.Entities.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>()
            .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>();

        CreateMap<BusinessLogic.Entities.Transferwarehouse, DataAccess.Entities.Transferwarehouse>()
            .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>();

        // Warehouse
        CreateMap<DataAccess.Entities.Warehouse, BusinessLogic.Entities.Warehouse>()
            .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>();

        CreateMap<BusinessLogic.Entities.Warehouse, DataAccess.Entities.Warehouse>()
            .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>();

        // WarehouseNextHops
        CreateMap<DataAccess.Entities.WarehouseNextHops, BusinessLogic.Entities.WarehouseNextHops>();
        CreateMap<BusinessLogic.Entities.WarehouseNextHops, DataAccess.Entities.WarehouseNextHops>();

        // Truck
        CreateMap<BusinessLogic.Entities.Truck, DataAccess.Entities.Truck>()
            .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>()
            .ForMember(dest => dest.Region,opt=>opt.ConvertUsing<GeoJsonConverter, string>(p=>p.RegionGeoJson));

        CreateMap<DataAccess.Entities.Truck, BusinessLogic.Entities.Truck>()
            .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>()
            .ForMember(dest => dest.RegionGeoJson,opt=>opt.ConvertUsing<GeoJsonConverter, Geometry>(p=>p.Region));

     }
}