using System.Net;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;


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

    public TransitionLogic(){
        _parcelRepository = new ParcelRepository(new DbContext());
    }
    public TransitionLogic(IParcelRepository parcelRepository){
        _parcelRepository = parcelRepository;
    }

    public object TransitionParcel(string trackingId, Parcel parcel)
    {
        // Validate trackingId and parcel
        if (!_trackingIDValidator.Validate(trackingId).IsValid || !_transitionValidator.Validate(parcel).IsValid){
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error.",
            }; 
        }

        // TODO: Check if parcel already exists
        // if(true){
        //     return new Error(){
        //         StatusCode = 409,
        //         ErrorMessage = "A parcel with the specified trackingID is already in the system.",
        //     }; 
        // }

        return new Parcel(){ TrackingId = trackingId };
    }
}
