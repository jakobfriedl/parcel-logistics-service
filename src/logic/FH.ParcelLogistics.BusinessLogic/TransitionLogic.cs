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
    private readonly ILogger<ITransitionLogic> _logger;

    public TransitionLogic(IParcelRepository parcelRepository, IMapper mapper, ILogger<ITransitionLogic> logger){
        _parcelRepository = parcelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Parcel TransitionParcel(string trackingId, Parcel parcel)
    {
        _logger.LogDebug($"TransitionParcel: [trackingId:{trackingId}]");
        // Validate trackingId and parcel
        if (!_trackingIDValidator.Validate(trackingId).IsValid || !_transitionValidator.Validate(parcel).IsValid){
            _logger.LogError($"TransitionParcel: [trackingId:{trackingId}] - Invalid parcel or trackingId");
            throw new BLValidationException("The operation failed due to an error.");
        }
        _logger.LogDebug($"TransitionParcel: {trackingId} - Validation successful");

        try{
            _logger.LogDebug($"TransitionParcel: [trackingId:{trackingId}] - Checking if parcel exists in database");
            var parcelById = _parcelRepository.GetByTrackingId(trackingId);
            _logger.LogDebug($"TransitionParcel: {trackingId} - Parcel found");
        } catch (DALNotFoundException e){
            _logger.LogDebug($"TransitionParcel: [trackingId:{trackingId}] - Inserting new parcel into database");
            parcel.TrackingId = trackingId;
            _logger.LogDebug($"TransitionParcel: [trackingId:{trackingId}] - TrackingId set on parcel");
            parcel.State = Parcel.ParcelState.Pickup;
            _logger.LogDebug($"TransitionParcel: [trackingId:{trackingId}] - Parcel state set to state:Pickup");
            var dbParcel = _mapper.Map<DataAccess.Entities.Parcel>(parcel);
            _logger.LogDebug($"TransitionParcel: {trackingId} - Parcel business layer entity mapped to DAL entity. [parcel:{parcel}] -> [dbParcel:{dbParcel}]");
            _parcelRepository.Submit(dbParcel);
            _logger.LogDebug($"TransitionParcel: [trackingId:{trackingId}] - Parcel submitted");
            return parcel; 
        }
        _logger.LogError($"TransitionParcel: Parcel with [trackingId:{trackingId}] already exists in the system.");
        throw new BLConflictException("A parcel with the specified trackingID is already in the system.");
    }
}
