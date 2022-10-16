using NUnit.Framework;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FluentValidation;
using FluentValidation.TestHelper;
using System.Net;

namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class SubmissionLogicTests
{
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
            PostalCode = "A-1010",
            City = "St. Pölten",
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
     private Parcel invalidParcel = new Parcel()
    {
        Weight = 0.0f,
        Sender = new Recipient()
        {
            Name = "John. Hermann Doe",
            Street = "Straße 1",
            PostalCode = "A-1010",
            City = "Wien!",
            Country = "Germany"
        },
        Recipient = new Recipient()
        {
            Name = "Jane Jr. Doe",
            Street = "Straße-!@ 2",
            PostalCode = "A-1150",
            City = "Wien 2",
            Country = "Austria"
        }
    };

    private SubmissionValidator submissionValidator;

    [SetUp]
    public void Setup()
    {
        submissionValidator = new SubmissionValidator();
    }

    [Test]
    public void SubmissionValidator()
    {
        //arrange

        //act
        var validParcelResult = submissionValidator.TestValidate(validParcel);
        var validRecipientParcelResult = submissionValidator.TestValidate(validRecipientParcel);
        var invalidParcelResult = submissionValidator.TestValidate(invalidParcel);

        //assert
        validParcelResult.ShouldNotHaveAnyValidationErrors();
        validRecipientParcelResult.ShouldNotHaveAnyValidationErrors();
        invalidParcelResult.ShouldHaveAnyValidationError();
    }

    [Test]
    public void SubmissionLogic()
    {
        //arrange
        var validParcelReturn = new Parcel() {
            TrackingId = "this_will_be_newly_generated"      
        };
        var submissionLogic = new SubmissionLogic();

        //act
        var invalidParcelResult = submissionLogic.SubmitParcel(invalidParcel);

        //assert
        Assert.AreNotEqual(invalidParcelResult, validParcelReturn);        
    }
}