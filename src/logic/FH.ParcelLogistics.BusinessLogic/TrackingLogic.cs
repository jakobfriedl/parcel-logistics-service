using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FluentValidation;

namespace FH.ParcelLogistics.BusinessLogic;

public class TrackingStateValidator : AbstractValidator<string>
{
    public TrackingStateValidator()
    {
        RuleFor(parcelTrackingId => parcelTrackingId).NotEmpty().Matches(@"^[A-Z0-9]{9}$");
    }
}


public class TrackingLogic : ITrackingLogic
{
    public object TrackParcel(string trackingId)
    {
        // TODO: Validate trackingId and return TrackingInformation, otherwise return error
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


