namespace FH.ParcelLogistics.Services.MappingProfiles;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;

[ExcludeFromCodeCoverage]
public class ParcelProfile : Profile
{
    public ParcelProfile(){

        CreateMap<DTOs.Recipient, BusinessLogic.Entities.Recipient>().ReverseMap();

        CreateMap<DTOs.Parcel, BusinessLogic.Entities.Parcel>()
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient))
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
            .ReverseMap();

        CreateMap<BusinessLogic.Entities.Parcel, DTOs.NewParcelInfo>()
            .ForMember(dest => dest.TrackingId, opt => opt.MapFrom(src => src.TrackingId))
            .ReverseMap();

        CreateMap<BusinessLogic.Entities.Parcel, DTOs.TrackingInformation>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.VisitedHops, opt => opt.MapFrom(src => src.VisitedHops))
            .ForMember(dest => dest.FutureHops, opt => opt.MapFrom(src => src.FutureHops))
            .ReverseMap();

        CreateMap<BusinessLogic.Entities.Recipient, DataAccess.Entities.Recipient>().ReverseMap(); 
        CreateMap<BusinessLogic.Entities.Parcel, DataAccess.Entities.Parcel>().ReverseMap(); 

        // Webhooks
        CreateMap<BusinessLogic.Entities.WebhookResponse, DTOs.WebhookResponse>().ReverseMap();
        CreateMap<BusinessLogic.Entities.WebhookResponse, DataAccess.Entities.WebhookResponse>().ReverseMap();

        CreateMap<DTOs.WebhookMessage, BusinessLogic.Entities.Parcel>().ReverseMap();
    }
}
