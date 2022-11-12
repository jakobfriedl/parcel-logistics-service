namespace FH.ParcelLogistics.BusinessLogic;

using System.Data;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class ReportTrackingIDValidator : AbstractValidator<string>
{
    public ReportTrackingIDValidator(){
        RuleFor(parcelTrackingId => parcelTrackingId).NotNull().Matches(@"^[A-Z0-9]{9}$");
    }
}

public class ReportHopValidator : AbstractValidator<string>
{
    public ReportHopValidator(){
        RuleFor(hopArrivalCode => hopArrivalCode).Matches(@"^[A-Z]{4}\d{1,4}$");
    }
}

public class ReportingLogic : IReportingLogic
{
    private readonly ReportTrackingIDValidator _reportTrackingIDValidator = new ReportTrackingIDValidator();
    private readonly ReportHopValidator _hopValidator = new ReportHopValidator();
    private readonly IParcelRepository _parcelRepository; 
    private readonly IHopRepository _hopRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReportingLogic> _logger;

    public ReportingLogic(IParcelRepository parcelRepository, IHopRepository hopRepository, IMapper mapper, ILogger<ReportingLogic> logger){
        _parcelRepository = parcelRepository;
        _hopRepository = hopRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public object ReportParcelDelivery(string trackingId)
    {
        _logger.LogDebug("ReportParcelDelivery called with trackingId: {trackingId}", trackingId);
        // Validate trackingId
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid){
            _logger.LogDebug("ReportParcelDelivery failed with trackingId: {trackingId}", trackingId);
            _logger.LogError("ReportParcelDelivery failed with trackingId: {trackingId}", trackingId);
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            }; 
        }
        _logger.LogDebug("ReportParcelDelivery validated trackingId: {trackingId}", trackingId);
        // Get parcel with supplied tracking id and update state, if tracking id does not exist, return 404
        try{
            _logger.LogDebug("ReportParcelDelivery getting parcel with trackingId: {trackingId}", trackingId);
            var parcel = _parcelRepository.GetByTrackingId(trackingId); 
            _logger.LogDebug("ReportParcelDelivery got parcel with trackingId: {trackingId}", trackingId);
            parcel.State = DataAccess.Entities.Parcel.ParcelState.Delivered; 
            var updatedParcel = _parcelRepository.Update(parcel); 
            _logger.LogDebug("ReportParcelDelivery updated parcel with trackingId: {trackingId}", trackingId);
        } catch(InvalidOperationException){
            _logger.LogDebug("ReportParcelDelivery failed with trackingId: {trackingId}", trackingId);
            _logger.LogError("Parcel does not exist with this tracking ID: {trackingId}", trackingId);
            return new Error(){
                StatusCode = 404,
                ErrorMessage = "Parcel does not exist with this tracking ID."
            }; 
        }
        _logger.LogDebug("ReportParcelDelivery succeeded with trackingId: {trackingId}", trackingId);
        return "Successfully reported hop.";
    }

    public object ReportParcelHop(string trackingId, string code)
    {
        _logger.LogDebug("ReportParcelHop called with trackingId: {trackingId} and code: {code}", trackingId, code);
        // Validate trackingId and code
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid || !_hopValidator.Validate(code).IsValid){
            _logger.LogDebug("ReportParcelHop failed with trackingId: {trackingId} and code: {code}", trackingId, code);
            _logger.LogError("ReportParcelHop failed with trackingId: {trackingId} and code: {code}", trackingId, code);
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            }; 
        }
        _logger.LogDebug("ReportParcelHop validated trackingId: {trackingId} and code: {code}", trackingId, code);

        try {
            _logger.LogDebug("ReportParcelHop getting parcel with trackingId: {trackingId}", trackingId);
            var parcel = _parcelRepository.GetByTrackingId(trackingId);
            _logger.LogDebug("ReportParcelHop got parcel with trackingId: {trackingId}", trackingId);
            parcel.State = DataAccess.Entities.Parcel.ParcelState.InTransport;
            _logger.LogDebug("ReportParcelHop updating parcel with trackingId: {trackingId}", trackingId);
        } catch (InvalidOperationException){
            _logger.LogDebug("ReportParcelHop failed with trackingId: {trackingId} and code: {code}", trackingId, code);
            _logger.LogError("Parcel does not exist with this tracking ID: {trackingId}", trackingId);
            return new Error(){
                StatusCode = 404,
                ErrorMessage = "Parcel does not exist with this tracking ID or hop with code not found."
            }; 
        }
        _logger.LogDebug("ReportParcelHop succeeded with trackingId: {trackingId} and code: {code}", trackingId, code);
        return "Successfully reported hop."; 
    }
}