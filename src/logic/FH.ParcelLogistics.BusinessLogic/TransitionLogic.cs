using System.Net;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FH.ParcelLogistics.BusinessLogic;

public class TransitionTrackingIDValidator : AbstractValidator<string>
{
    public TransitionTrackingIDValidator()
    {
        RuleFor(parcelTrackingId => parcelTrackingId).NotNull().Matches(@"^[A-Z0-9]{9}$");
    }
}

//validate Data
public class TransitionValidator : AbstractValidator<Parcel>
{
    public TransitionValidator()
    {
        //check Recipient
        RuleFor(parcel => parcel.Recipient).NotNull();
        RuleFor(recipeintCountry => recipeintCountry.Recipient.Country).NotNull().Matches(@"Austria|Österreich").DependentRules(() => {
            //only Checks if Country is Austria|Österreich  
            RuleFor(recipientPostalCode => recipientPostalCode.Recipient.PostalCode).NotNull().Length(6).Matches(@"[A][-]\d{4}");
            RuleFor(recipientCity => recipientCity.Recipient.City).NotNull().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
            RuleFor(recipientStreet => recipientStreet.Recipient.Street).NotNull().Matches(@"^[A-Z][a-zäüöß /\d-]*");
            RuleFor(recipientName => recipientName.Recipient.Name).NotNull().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
        });

        //check Sender
        RuleFor(parcel => parcel.Sender).NotNull();
        RuleFor(senderCountry => senderCountry.Sender.Country).NotNull().Matches(@"Austria|Österreich").DependentRules(() => {
            //only Checks if Country is Austria|Österreich
            RuleFor(senderPostalCode => senderPostalCode.Sender.PostalCode).NotNull().Length(6).Matches(@"[A][-]\d{4}");
            RuleFor(senderCity => senderCity.Sender.City).NotNull().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
            RuleFor(senderStreet => senderStreet.Sender.Street).NotNull().Matches(@"^[a-zA-Z][a-zäüöß /\d-]*");
            RuleFor(senderName => senderName.Sender.Name).NotNull().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
        });
        RuleFor(parcelWeight => parcelWeight.Weight).NotNull().GreaterThan(0.0f);
    }
}

public class TransitionLogic : ITransitionLogic
{
    private readonly TransitionTrackingIDValidator _trackingIDValidator = new TransitionTrackingIDValidator();
    private readonly TransitionValidator _transitionValidator = new TransitionValidator();
    private readonly IParcelRepository _parcelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TransitionLogic> _logger;

    public TransitionLogic(IParcelRepository parcelRepository, IMapper mapper, ILogger<TransitionLogic> logger){
        _parcelRepository = parcelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public object TransitionParcel(string trackingId, Parcel parcel)
    {
        _logger.LogDebug("TransitionParcel: {trackingId}", trackingId);
        // Validate trackingId and parcel
        if (!_trackingIDValidator.Validate(trackingId).IsValid || !_transitionValidator.Validate(parcel).IsValid){
            _logger.LogDebug("TransitionParcel: {trackingId} - Validation failed", trackingId);
            _logger.LogWarning("TransitionParcel: {trackingId} - Validation failed", trackingId);
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error.",
            }; 
        }
        _logger.LogDebug("TransitionParcel: {trackingId} - Validation successful", trackingId);

        try{
            _logger.LogDebug("TransitionParcel: {trackingId} - Try to get parcel", trackingId);
            var parcelById = _parcelRepository.GetByTrackingId(trackingId);
            _logger.LogDebug("TransitionParcel: {trackingId} - Parcel found", trackingId);
        } catch (InvalidOperationException e){
            if (e.Message == "Sequence contains no elements"){
                _logger.LogDebug("TransitionParcel: {trackingId} - Parcel found for Transition", trackingId);
                parcel.TrackingId = trackingId;
                _logger.LogDebug("TransitionParcel: {trackingId} - Try to add parcel", trackingId);
                parcel.State = Parcel.ParcelState.Pickup;
                _logger.LogDebug("TransitionParcel: {trackingId} - Parcel added", trackingId);
                var dbParcel = _mapper.Map<DataAccess.Entities.Parcel>(parcel);
                _logger.LogDebug("TransitionParcel: {trackingId} - Try to save parcel", trackingId);
                _parcelRepository.Submit(dbParcel);
                _logger.LogDebug("TransitionParcel: {trackingId} - Parcel saved", trackingId);
                return parcel; 
            }
        }
        _logger.LogDebug("TransitionParcel: {trackingId} - Parcel not found for Transition", trackingId);
        _logger.LogError("TransitionParcel: {trackingId} - Parcel not found for Transition", trackingId);
        return new Error(){
            StatusCode = 409,
            ErrorMessage = "A parcel with the specified trackingID is already in the system.",
        }; 
    }
}
