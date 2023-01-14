namespace FH.ParcelLogistics.BusinessLogic;

using System.Data;
using System.Text;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FH.ParcelLogistics.WebhookManager.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
    private readonly IWebhookManager _webhookManager;
    private readonly HttpClient _httpClient;

    public ReportingLogic(IParcelRepository parcelRepository, IHopRepository hopRepository, IMapper mapper, ILogger<IReportingLogic> logger, IWebhookManager webhookManager, HttpClient httpClient){
        _parcelRepository = parcelRepository;
        _hopRepository = hopRepository;
        _mapper = mapper;
        _logger = logger;
        _webhookManager = webhookManager;
        _httpClient = httpClient;
    }

    public async Task ReportParcelDelivery(string trackingId)
    {
        _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}]");
        // Validate trackingId
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid){
            _logger.LogError($"ReportParcelDelivery: [trackingId:{trackingId}] - Invalid trackingId");
            throw new BLValidationException("The operation failed due to an error.");
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

            // Notify webhook subscribers
            _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Notifying webhook subscribers");
            try {
                await _webhookManager.Notify(parcel.TrackingId);
            } catch (Exception e) {
                _logger.LogError(e.Message);
                throw new BLException("The operation failed due to an error.", e);
            }
            _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Webhook subscribers notified"); 

        } catch(DALNotFoundException e){
            _logger.LogError($"ReportParcelDelivery: [trackingId:{trackingId}] - Parcel was not found in database");
            throw new BLNotFoundException("Parcel does not exist with this tracking ID.", e);
        }
        _logger.LogDebug($"ReportParcelDelivery: [trackingId:{trackingId}] - Parcel delivery reported");
    }

    public async Task ReportParcelHop(string trackingId, string code)
    {
        _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], code: {code}");
        // Validate trackingId and code
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid || !_hopValidator.Validate(code).IsValid){
            _logger.LogError($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}] - Invalid trackingId or code");
            throw new BLValidationException("The operation failed due to an error.");
        }
        _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}] - Validated trackingId and code");

        try {
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Checking if parcel exists in database");
            var parcel = _parcelRepository.GetByTrackingId(trackingId);
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel with trackingId:{trackingId} found in database");

            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Checking if hop exists in database");
            var hop = _hopRepository.GetByCode(code);
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Hop with code:{code} found in database");

            // Remove hop from future hops
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Searching for hop in future hops");
            var hopArrival = parcel.FutureHops.Find(_ => _.Code == code);
            if (hopArrival is null) {
                _logger.LogError($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Hop with code {code} does not exist in future hops");
                throw new BLNotFoundException("Hop with this code does not exist in future hops.");
            }
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Removing hop from future hops");
            parcel.FutureHops.Remove(hopArrival); 
            
            // Add hop to visited hops
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Adding hop to visited hops");
            parcel.VisitedHops.Add(hopArrival);

            // Update parcel state
            if (hop.HopType == "warehouse"){
                parcel.State = DataAccess.Entities.Parcel.ParcelState.InTransport;
            } else if (hop.HopType == "truck") {
                parcel.State = DataAccess.Entities.Parcel.ParcelState.InTruckDelivery; 
            } else if (hop.HopType == "transferwarehouse") {
                var transferwarehouse = hop as FH.ParcelLogistics.DataAccess.Entities.Transferwarehouse;
                var request = new HttpRequestMessage(HttpMethod.Post, $"{transferwarehouse.LogisticsPartnerUrl}/parcel/{trackingId}");
                request.Content = new StringContent(JsonConvert.SerializeObject(parcel), Encoding.UTF8, "application/json");
                await _httpClient.SendAsync(request);
                parcel.State = DataAccess.Entities.Parcel.ParcelState.Transferred;
            }
            
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel with trackingId:{trackingId} updated to state:{parcel.State}");
            var updatedParcel = _parcelRepository.Update(parcel);
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel updated in database");

            // Notify webhook subscribers
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Notifying webhook subscribers");
            try {
                await _webhookManager.Notify(parcel.TrackingId);
            } catch (Exception e) {
                _logger.LogError(e.Message);
            }
            _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Webhook subscribers notified");

        } catch (DALNotFoundException e){
            _logger.LogError($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel was not found in database");
            throw new BLNotFoundException("Parcel does not exist with this tracking ID or hop with code not found.", e);
        }
        _logger.LogDebug($"ReportParcelHop: [trackingId:{trackingId}], [code:{code}]  - Parcel hop reported");
    }
}