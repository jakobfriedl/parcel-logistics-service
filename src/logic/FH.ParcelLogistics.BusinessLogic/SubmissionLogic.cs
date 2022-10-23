using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;

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
    public SubmissionLogic(){
        _parcelRepository = new ParcelRepository();
    }
    public SubmissionLogic(IParcelRepository parcelRepository){
        _parcelRepository = parcelRepository;
    }

    public object SubmitParcel(Parcel parcel)
    {
        // Validate parcel
        if (!_submissionValidator.Validate(parcel).IsValid){
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            };
        }

        // TODO: Check if sender and receiver exist
        // if (...){
        //     return new Error(){
        //         StatusCode = 404,
        //         ErrorMessage = "The address of sender or receiver was not found."
        //     }
        // }

        return new Parcel() {
            TrackingId = "this_will_be_newly_generated"
        };
    }
}
