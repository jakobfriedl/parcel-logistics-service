using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using System.Net;
using FluentValidation;

namespace FH.ParcelLogistics.BusinessLogic;

public class TrackingStateValidator : AbstractValidator<string>
{
    public TrackingStateValidator(){
        RuleFor(parcelTrackingId => parcelTrackingId).NotNull().Matches(@"^[A-Z0-9]{9}$");
    }
}


public class TrackingLogic : ITrackingLogic
{
    TrackingStateValidator trackingStateValidator = new TrackingStateValidator();
    public object TrackParcel(string trackingId)
    {
        // Validate trackingId
        if (!trackingStateValidator.Validate(trackingId).IsValid){
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error.",
            };
        }

        // TODO: Check if parcel exists
        // if (...){
        //     return new Error(){
        //         StatusCode = 404,
        //         ErrorMessage = "Parcel does not exist with this tracking ID.",
        //     };
        // }

        // TODO: Change hardcoded return type
        return new Parcel()
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
    }
}


