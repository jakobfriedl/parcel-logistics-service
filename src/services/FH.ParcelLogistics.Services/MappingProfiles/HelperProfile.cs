namespace FH.ParcelLogistics.Services.MappingProfiles;
using AutoMapper;

public class HelperProfile : Profile
{
    public HelperProfile(){
        CreateMap<DTOs.Error, BusinessLogic.Entities.Error>().ReverseMap();
    }
}
