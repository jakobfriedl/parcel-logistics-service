using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using System.Net;
using FluentValidation;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;

namespace FH.ParcelLogistics.BusinessLogic;

public class TrackingStateValidator : AbstractValidator<string>
{
    public TrackingStateValidator(){
        RuleFor(parcelTrackingId => parcelTrackingId).NotNull().Matches(@"^[A-Z0-9]{9}$");
    }
}


public class TrackingLogic : ITrackingLogic
{
    private readonly TrackingStateValidator _trackingStateValidator = new TrackingStateValidator();
    private readonly IParcelRepository _parcelRepository;

    public TrackingLogic(){
        _parcelRepository = new ParcelRepository();
    }
    public TrackingLogic(IParcelRepository parcelRepository){
        _parcelRepository = parcelRepository;
    }

    public object TrackParcel(string trackingId)
    {
        // Validate trackingId
        if (!_trackingStateValidator.Validate(trackingId).IsValid){
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


