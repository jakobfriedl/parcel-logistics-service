using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace FH.ParcelLogistics.BusinessLogic;

//validate Data
public class ParcelValidator : AbstractValidator<Parcel>
{
    public ParcelValidator()
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
            RuleFor(senderStreet => senderStreet.Sender.Street).NotEmpty().Matches(@"^[A-Z][a-zäüöß /\d-]*");
            RuleFor(senderName => senderName.Sender.Name).NotEmpty().Matches(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*");
        });
        RuleFor(hopArrivalCode => hopArrivalCode.FutureHops).SetInheritanceValidator(i => new HopArrivalValidator());
        RuleFor(parcelTrackingId => parcelTrackingId.TrackingId).NotEmpty().Matches(@"^[A-Z0-9]{9}$");
        RuleFor(parcelWeight => parcelWeight.Weight).NotEmpty().GreaterThan(0.0f);
        RuleFor(visitedHops => visitedHops.VisitedHops).NotEmpty();
        RuleFor(futureHops => futureHops.FutureHops).NotEmpty();
        RuleFor(parcelState => parcelState.State).NotEmpty();
        RuleFor(parcelState => parcelState.State).IsInEnum();
    }
}

public class HopArrivalValidator : AbstractValidator<HopArrival>
{
    public HopArrivalValidator()
    {
        RuleFor(hopArrival => hopArrival.Code).NotEmpty().Matches(@"^[A-Z]{4}\\d{1,4}$");
        RuleFor(hopArrivalDescription => hopArrivalDescription.Description).NotEmpty().Matches(@"[A-Za-z\d äöüÄÖÜß-]+");
    }
}

public class WarehouseDescriptionValidator : AbstractValidator<Warehouse>
{
    public WarehouseDescriptionValidator()
    {
        RuleFor(warehouseDescription => warehouseDescription.Description).NotEmpty().Matches(@"[A-Za-z\d äöüÄÖÜß-]+");
    }
}


public class ReceivingLogic : IReceivingLogic
{
    // ParcelValidator parcelValidator = new ParcelValidator();
    // Parcel parcel = new Parcel();
    // ValidationResult result = ParcelValidator.Validate(parcel);
}
