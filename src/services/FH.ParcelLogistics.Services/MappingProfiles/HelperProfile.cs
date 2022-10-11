namespace FH.ParcelLogistics.Services.MappingProfiles;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;

[ExcludeFromCodeCoverage]
public class HelperProfile : Profile
{
    public HelperProfile(){
        CreateMap<DTOs.Error, BusinessLogic.Entities.Error>().ReverseMap();
    }
}
