using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using NetTopologySuite.Geometries;

namespace FH.ParcelLogistics.Services.MappingProfiles;

[ExcludeFromCodeCoverage]
public class GeoProfile : Profile
{
    public GeoProfile()
    {
        CreateMap<BusinessLogic.Entities.GeoCoordinate, Point>()
            .ConvertUsing<GeoPointConverter>();
        CreateMap<Point, BusinessLogic.Entities.GeoCoordinate>()
            .ConvertUsing<GeoPointConverter>();

        CreateMap<string, Geometry>()
            .ForMember(dest => dest, opt => opt.ConvertUsing<GeoJsonConverter, string>())
            .ReverseMap();
    }
}

