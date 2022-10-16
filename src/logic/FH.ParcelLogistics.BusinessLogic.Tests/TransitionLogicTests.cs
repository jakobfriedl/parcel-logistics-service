using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class TransitionLogicTests
{
    private string validTrackingId = "ABCD34590";
    private string invalidTrackingId = "ABCD!æ34590§$%&";
    private TransitionTrackingIDValidator transitionTrackingIDValidator;
    private TransitionValidator transitionValidator;

    private Parcel validParcel = new Parcel()
    {
        Weight = 1.1f,
        Sender = new Recipient()
        {
            Name = "John Doe",
            Street = "Straße 1",
            PostalCode = "A-1010",
            City = "Wien",
            Country = "Austria"
        },
        Recipient = new Recipient()
        {
            Name = "Jane Doe",
            Street = "Straße 2",
            PostalCode = "A-1150",
            City = "Wien",
            Country = "Austria"
        }
    };

    private Parcel validRecipientParcel = new Parcel()
    {
        Weight = 0.1f,
        Sender = new Recipient()
        {
            Name = "John Doe",
            Street = "Straße 1",
            PostalCode = "1010",
            City = "St. Pölten",
            Country = "Germany"
        },
        Recipient = new Recipient()
        {
            Name = "Jane Doe",
            Street = "Straße 2",
            PostalCode = "A-1150",
            City = "Wien",
            Country = "Austria"
        }
    };
    private Parcel invalidParcel = new Parcel()
    {
        Weight = 0.0f,
        Sender = new Recipient()
        {
            Name = "John. Hermann Doe",
            Street = "Straße 1",
            PostalCode = "1010",
            City = "Wien!",
            Country = "Austria"
        },
        Recipient = new Recipient()
        {
            Name = "Jane Jr. Doe",
            Street = "Straße-!@ 2",
            PostalCode = "AT-1150",
            City = "Wien 2",
            Country = "Austria"
        }
    };

    [SetUp]
    public void Setup()
    {
        transitionTrackingIDValidator = new TransitionTrackingIDValidator();
        transitionValidator = new TransitionValidator();
    }

    [Test]
    public void TransitionTrackingIDValidator()
    {
        // arrange

        // act
        var validIDResult = transitionTrackingIDValidator.TestValidate(validTrackingId);
        var invalidIDResult = transitionTrackingIDValidator.TestValidate(invalidTrackingId);

        // assert
        validIDResult.ShouldNotHaveAnyValidationErrors();
        invalidIDResult.ShouldHaveAnyValidationError();
    }

    [Test]
    public void TransitionValidator()
    {
        // arrange

        // act
        var validParcelResult = transitionValidator.TestValidate(validParcel);
        var RecipientParcelResult = transitionValidator.TestValidate(validRecipientParcel);
        var invalidParcelResult = transitionValidator.TestValidate(invalidParcel);

        // assert
        validParcelResult.ShouldNotHaveAnyValidationErrors();
        RecipientParcelResult.ShouldHaveAnyValidationError();
        invalidParcelResult.ShouldHaveAnyValidationError();
    }

}