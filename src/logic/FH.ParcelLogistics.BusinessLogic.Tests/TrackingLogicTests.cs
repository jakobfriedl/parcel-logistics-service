using NUnit.Framework;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FluentValidation;
using FluentValidation.TestHelper;

namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class TrackingLogicTests
{

    private string validParcelTrackingId = "ABCD34590";
    private string invalidParcelTrackingId = "ABCD!æ34590§$%&";
    private TrackingStateValidator trackingStateValidator;

    [SetUp]
    public void Setup()
    {
        trackingStateValidator = new TrackingStateValidator();
    }

    [Test]
    public void TrackingStateValidator()
    {
        // arrange
        
        // act
        var validResult = trackingStateValidator.TestValidate((validParcelTrackingId));
        var invalidResult = trackingStateValidator.TestValidate((invalidParcelTrackingId));

        // assert
        validResult.ShouldNotHaveAnyValidationErrors();
        invalidResult.ShouldHaveAnyValidationError();
    }

    [Test]
    public void TrackParcel()
    {
        // arrange
        var trackingLogic = new TrackingLogic();
        var validReturn = new Parcel()
        {
            State = Parcel.ParcelState.Pickup,
            VisitedHops = new List<HopArrival>(){
                new HopArrival() {
                    Code = "TEST1",
                    Description = "Test1",
                    DateTime = DateTime.Now,
                },
                new HopArrival() {
                    Code = "TEST2",
                    Description = "Test2",
                    DateTime = DateTime.Now,
                },
            },
            FutureHops = new List<HopArrival>(){
                new HopArrival() {
                    Code = "TEST3",
                    Description = "Test3",
                    DateTime = DateTime.Now,
                },
                new HopArrival() {
                    Code = "TEST4",
                    Description = "Test4",
                    DateTime = DateTime.Now,
                }
            },
        };
        
        // act
        var invalidResult = trackingLogic.TrackParcel(validParcelTrackingId);

        // assert
        Assert.AreNotEqual(invalidResult, validReturn);
    }
}