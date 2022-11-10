using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using System.Net;
using FluentValidation;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using AutoMapper;

namespace FH.ParcelLogistics.BusinessLogic;

public class TrackingStateValidator : AbstractValidator<string>
{
    public TrackingStateValidator(){
        RuleFor(parcelTrackingId => parcelTrackingId).NotNull().Matches(@"^[A-Z0-9]{9}$");
    }
}


public class TrackingLogic : ITrackingLogic
{
    private readonly TrackingStateValidator _trackingStateValidator = new TrackingStateValidator();
    private readonly IParcelRepository _parcelRepository;
    private readonly IMapper _mapper;
    
    public TrackingLogic(IParcelRepository parcelRepository, IMapper mapper){
        _parcelRepository = parcelRepository;
        _mapper = mapper;
    }

    public object TrackParcel(string trackingId)
    {
        // Validate trackingId
        if (!_trackingStateValidator.Validate(trackingId).IsValid){
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error.",
            };
        }

        // Get parcel with supplied tracking id, if tracking id does not exist, return 404
        try{
            var parcel = _parcelRepository.GetByTrackingId(trackingId);
            return _mapper.Map<Parcel>(parcel);
        } catch(InvalidOperationException){
            return new Error(){
                StatusCode = 404,
                ErrorMessage = "Parcel does not exist with this tracking ID.",
            };
        }
    }
}


