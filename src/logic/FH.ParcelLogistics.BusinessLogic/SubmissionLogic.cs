using System.Data;
using AutoMapper;
using AutoMapper;
using BingMapsRESTToolkit;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace FH.ParcelLogistics.BusinessLogic;

public class SubmissionValidator : AbstractValidator<Parcel>
{
    public SubmissionValidator()
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
            RuleFor(senderStreet => senderStreet.Sender.Street).NotNull().Matches(@"^[A-Z][a-zäüöß /\d-]*");
            RuleFor(senderName => senderName.Sender.Name).NotNull().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
        });
        RuleFor(parcelWeight => parcelWeight.Weight).NotNull().GreaterThan(0.0f);
    }
}

public class SubmissionLogic : ISubmissionLogic
{
    private readonly SubmissionValidator _submissionValidator = new SubmissionValidator();
    private readonly IParcelRepository _parcelRepository;
    private readonly IMapper _mapper; 
    private readonly ILogger<ISubmissionLogic> _logger;

    public SubmissionLogic(IParcelRepository parcelRepository, IMapper mapper, ILogger<ISubmissionLogic> logger){
        _parcelRepository = parcelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    private string GenerateValidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        return idGenerator.Generate();
    }

    public Parcel SubmitParcel(Parcel parcel)
    {
        _logger.LogDebug($"SubmitParcel: [parcel:{parcel}]");
        // Validate parcel
        if (!_submissionValidator.Validate(parcel).IsValid){
            _logger.LogError($"SubmitParcel: [parcel:{parcel}] - Invalid parcel");
            throw new BLValidationException("The operation failed due to an error.");
        }
        _logger.LogDebug($"SubmitParcel: [parcel:{parcel}] - Valid parcel");

        // Generate TrackingId
        var trackingId = "";
        do {
            trackingId = GenerateValidTrackingId();
        } while(_parcelRepository.TryGetByTrackingId(trackingId, out _));
        parcel.TrackingId = trackingId;

        _logger.LogDebug($"SubmitParcel: Generated TrackingId {parcel.TrackingId} for [parcel:{parcel}]");
        parcel.State = Parcel.ParcelState.Pickup; 
        _logger.LogDebug($"SubmitParcel: Set State to {parcel.State} for parcel {parcel}");

        var dbParcel = _mapper.Map<Parcel, DataAccess.Entities.Parcel>(parcel);
        _logger.LogDebug($"SubmitParcel: Mapped business layer entity to DAL entity. [parcel:{parcel}] -> [dbParcel:{dbParcel}]");
        try {
            var result = _parcelRepository.Submit(dbParcel); 
            _logger.LogDebug($"SubmitParcel: [parcel:{parcel}] - Parcel submitted");
            _logger.LogDebug($"SubmitParcel: [parcel:{parcel}] - Returning newly created parcel");
            return _mapper.Map<Parcel>(result); 
        } catch (DALException e){
            _logger.LogError($"SubmitParcel: [parcel:{parcel}] - Failed to submit parcel");
            throw new BLException("The operation failed due to an error.");
        }
    }
}
