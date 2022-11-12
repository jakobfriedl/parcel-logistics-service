using AutoMapper;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
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
    private readonly ILogger<SubmissionLogic> _logger;

    public SubmissionLogic(IParcelRepository parcelRepository, IMapper mapper, ILogger<SubmissionLogic> logger){
        _parcelRepository = parcelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    private string GenerateValidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        return idGenerator.Generate();
    }

    public object SubmitParcel(Parcel parcel)
    {
        _logger.LogDebug("SubmitParcel called with {parcel}", parcel);
        // Validate parcel
        if (!_submissionValidator.Validate(parcel).IsValid)
        {
            _logger.LogDebug("Parcel {parcel} is not valid", parcel);
            _logger.LogWarning("Parcel {parcel} is not valid", parcel);
            return new Error()
            {
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            };
        }
        _logger.LogDebug("Parcel {parcel} is valid", parcel);

        parcel.TrackingId = GenerateValidTrackingId();
        _logger.LogDebug("Generated TrackingId {trackingId} for parcel {parcel}", parcel.TrackingId, parcel);
        parcel.State = Parcel.ParcelState.Pickup; 
        _logger.LogDebug("Set State to {state} for parcel {parcel}", parcel.State, parcel);
        var dbParcel = _mapper.Map<Parcel, DataAccess.Entities.Parcel>(parcel);
        _logger.LogDebug("Mapped parcel {parcel} to dbParcel {dbParcel}", parcel, dbParcel);
        var result = _parcelRepository.Submit(dbParcel); 
        _logger.LogDebug("Submitted parcel {parcel} to dbParcel {dbParcel}", parcel, dbParcel);

        // TODO: Check if sender and receiver exist
        // if (...){
        //     return new Error(){
        //         StatusCode = 404,
        //         ErrorMessage = "The address of sender or receiver was not found."
        //     }
        // }

        _logger.LogDebug("Parcel {parcel} was submitted successfully", parcel);
        return _mapper.Map<Parcel>(result); 
    }
}
