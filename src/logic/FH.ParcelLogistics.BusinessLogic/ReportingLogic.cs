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
    private readonly ILogger<IReportingLogic> _logger;

    public ReportingLogic(IParcelRepository parcelRepository, IHopRepository hopRepository, IMapper mapper, ILogger<IReportingLogic> logger){
        _parcelRepository = parcelRepository;
        _hopRepository = hopRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public object ReportParcelDelivery(string trackingId)
    {
        _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}]");
        // Validate trackingId
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid){
            _logger.LogError($"ReportParcelDelivery: [trackingId:{trackingId}] - Invalid trackingId");
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            }; 
        }
        _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Validated trackingId");

        // Get parcel with supplied tracking id and update state, if tracking id does not exist, return 404
        try{
            _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Checking if parcel exists in database");
            var parcel = _parcelRepository.GetByTrackingId(trackingId); 
            _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Parcel with trackingId:{trackingId} found in database");
            parcel.State = DataAccess.Entities.Parcel.ParcelState.Delivered; 
            _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Parcel with trackingId:{trackingId} updated to state:Delivered");
            var updatedParcel = _parcelRepository.Update(parcel); 
            _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Parcel updated in database");
        } catch(InvalidOperationException){
            _logger.LogError($"ReportParcelDelivery: [trackingId:{trackingId}] - Parcel was not found in database");
            return new Error(){
                StatusCode = 404,
                ErrorMessage = "Parcel does not exist with this tracking ID."
            }; 
        }
        _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Parcel delivery reported");
        return "Successfully reported hop.";
    }

    public object ReportParcelHop(string trackingId, string code)
    {
        _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], code: {code}");
        // Validate trackingId and code
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid || !_hopValidator.Validate(code).IsValid){
            _logger.LogError($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}] - Invalid trackingId or code");
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            }; 
        }
        _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}] - Validated trackingId and code");

        try {
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Checking if parcel exists in database");
            var parcel = _parcelRepository.GetByTrackingId(trackingId);
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel with trackingId:{trackingId} found in database");
            parcel.State = DataAccess.Entities.Parcel.ParcelState.InTransport;
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel with trackingId:{trackingId} updated to state:InTransport");
        } catch (InvalidOperationException){
            _logger.LogError($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel was not found in database");
            return new Error(){
                StatusCode = 404,
                ErrorMessage = "Parcel does not exist with this tracking ID or hop with code not found."
            }; 
        }
        _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel hop reported");
        return "Successfully reported hop."; 
    }
}