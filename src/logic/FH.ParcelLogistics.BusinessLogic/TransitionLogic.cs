using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FluentValidation;


namespace FH.ParcelLogistics.BusinessLogic;

public class TransitionTrackingIDValidator : AbstractValidator<string>
{
    public TransitionTrackingIDValidator()
    {
        RuleFor(parcelTrackingId => parcelTrackingId).NotEmpty().Matches(@"^[A-Z0-9]{9}$");
    }
}

//validate Data
public class TransitionValidator : AbstractValidator<Parcel>
{
    public TransitionValidator()
    {
        //check Recipient
        RuleFor(parcel => parcel.Recipient).NotNull();
        RuleFor(recipeintCountry => recipeintCountry.Recipient.Country).NotEmpty().Matches(@"Austria|Österreich").DependentRules(() =>
        {
            //only Checks if Country is Austria|Österreich  
            RuleFor(recipientPostalCode => recipientPostalCode.Recipient.PostalCode).NotEmpty().Length(6).Matches(@"[A][-]\d{4}");
            RuleFor(recipientCity => recipientCity.Recipient.City).NotEmpty().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
            RuleFor(recipientStreet => recipientStreet.Recipient.Street).NotEmpty().Matches(@"^[A-Z][a-zäüöß /\d-]*");
            RuleFor(recipientName => recipientName.Recipient.Name).NotEmpty().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
        });

        //check Sender
        RuleFor(parcel => parcel.Sender).NotNull();
        RuleFor(senderCountry => senderCountry.Sender.Country).NotEmpty().Matches(@"Austria|Österreich").DependentRules(() =>
        {
            //only Checks if Country is Austria|Österreich
            RuleFor(senderPostalCode => senderPostalCode.Sender.PostalCode).NotEmpty().Length(6).Matches(@"[A][-]\d{4}");
            RuleFor(senderCity => senderCity.Sender.City).NotEmpty().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
            RuleFor(senderStreet => senderStreet.Sender.Street).NotEmpty().Matches(@"^[a-zA-Z][a-zäüöß /\d-]*");
            RuleFor(senderName => senderName.Sender.Name).NotEmpty().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
        });
        RuleFor(parcelWeight => parcelWeight.Weight).NotEmpty().GreaterThan(0.0f);
    }
}

public class TransitionLogic : ITransitionLogic
{
    public object TransitionParcel(string trackingId, Parcel parcel)
    {
        // TODO: Validate trackingId and Parcel, then return trackingId or error
        return new Parcel()
        {
            TrackingId = trackingId,
        };
    }
}
