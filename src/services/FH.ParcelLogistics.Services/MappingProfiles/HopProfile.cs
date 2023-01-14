namespace FH.ParcelLogistics.Services.MappingProfiles;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FH.ParcelLogistics.DataAccess.Entities;
using NetTopologySuite.Geometries;

[ExcludeFromCodeCoverage]
public class HopProfile : Profile
{
    public HopProfile(){
        // DTO Coordinate
        CreateMap<DTOs.GeoCoordinate, BusinessLogic.Entities.GeoCoordinate>().ReverseMap();

        // DTO Hop
        CreateMap<DTOs.Hop, BusinessLogic.Entities.Hop>().ReverseMap();

        // DTO HopArrival
        CreateMap<DTOs.HopArrival, BusinessLogic.Entities.HopArrival>().ReverseMap();

        // DTO Transferwarehouse
        CreateMap<DTOs.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>()
            .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>()
            .ReverseMap();

        // DTO WarehouseNextHops
        CreateMap<DTOs.WarehouseNextHops, BusinessLogic.Entities.WarehouseNextHops>().ReverseMap();

        // DTO Truck
        CreateMap<DTOs.Truck, BusinessLogic.Entities.Truck>()
            .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>()
            .ReverseMap();

        // DTO Warehouse
        CreateMap<DTOs.Warehouse, BusinessLogic.Entities.Warehouse>()
            .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>()
            .ReverseMap();

        // HopArrival
        CreateMap<BusinessLogic.Entities.HopArrival, DataAccess.Entities.HopArrival>().ReverseMap();

        // Hop
        CreateMap<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>().ReverseMap();

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

        // Transferwarehouse
        CreateMap<BusinessLogic.Entities.Transferwarehouse, DataAccess.Entities.Transferwarehouse>()
            .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>()
            .ForMember(dest => dest.Region,opt=>opt.ConvertUsing<GeoJsonConverter, string>(p=>p.RegionGeoJson));
        
        CreateMap<DataAccess.Entities.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>()
            .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>()
            .ForMember(dest => dest.RegionGeoJson,opt=>opt.ConvertUsing<GeoJsonConverter, Geometry>(p=>p.Region));

     }
}