namespace FH.ParcelLogistics.Services.MappingProfiles;
using AutoMapper;

public class ParcelProfile : Profile
{
    public ParcelProfile(){
        CreateMap<(DTOs.Parcel, DTOs.NewParcelInfo, DTOs.TrackingInformation), BusinessLogic.Entities.Parcel>()
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Item1.Weight))
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Item1.Recipient))
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Item1.Sender))
            .ForMember(dest => dest.TrackingId, opt => opt.MapFrom(src => src.Item2.TrackingId))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Item3.State))
            .ForMember(dest => dest.VisitedHops, opt => opt.MapFrom(src => src.Item3.VisitedHops))
            .ForMember(dest => dest.FutureHops, opt => opt.MapFrom(src => src.Item3.FutureHops))
            .ReverseMap();
    }
}
