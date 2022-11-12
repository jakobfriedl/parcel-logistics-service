using System.Net;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<ITrackingLogic> _logger;
    
    public TrackingLogic(IParcelRepository parcelRepository, IMapper mapper, ILogger<ITrackingLogic> logger){
        _parcelRepository = parcelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public object TrackParcel(string trackingId)
    {
        _logger.LogDebug($"TrackParcel called with trackingId: {trackingId}");
        // Validate trackingId
        if (!_trackingStateValidator.Validate(trackingId).IsValid){
            _logger.LogError($"TrackParcel failed with trackingId: {trackingId}");
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error.",
            };
        }
        _logger.LogDebug($"TrackParcel validated trackingId: {trackingId}");

        // Get parcel with supplied tracking id, if tracking id does not exist, return 404
        try{
            _logger.LogDebug($"TrackParcel getting parcel with trackingId: {trackingId}");
            var parcel = _parcelRepository.GetByTrackingId(trackingId);
            _logger.LogDebug($"TrackParcel got parcel with trackingId: {trackingId}");
            return _mapper.Map<Parcel>(parcel);
        } catch(InvalidOperationException){
            _logger.LogError($"TrackParcel failed with trackingId: {trackingId}");
            return new Error(){
                StatusCode = 404,
                ErrorMessage = "Parcel does not exist with this tracking ID.",
            };
        }
    }
}


