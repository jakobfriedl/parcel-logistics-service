using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FluentValidation;
namespace FH.ParcelLogistics.BusinessLogic;

public class ReportTrackingIDValidator : AbstractValidator<string>
{
    public ReportTrackingIDValidator(){
        RuleFor(parcelTrackingId => parcelTrackingId).NotNull().Matches(@"^[A-Z0-9]{9}$");
    }
}

public class ReportHopValidator : AbstractValidator<string>
{
    public ReportHopValidator(){
        RuleFor(hopArrivalCode => hopArrivalCode).Matches(@"^[A-Z]{4}\d{1,4}$");
    }
}

public class ReportingLogic : IReportingLogic
{
    ReportTrackingIDValidator reportTrackingIDValidator = new ReportTrackingIDValidator();
    ReportHopValidator hopValidator = new ReportHopValidator();
    public object ReportParcelDelivery(string trackingId)
    {
        // Validate trackingId
        if (!reportTrackingIDValidator.Validate(trackingId).IsValid){
            return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            }; 
        }

        // TODO: Check if parcel exists
        // if(...){
        //     return new Error(){
        //         StatusCode = 404,
        //         ErrorMessage = "Parcel does not exist with this tracking ID."
        //     }; 
        // }

        return "Successfully reported hop.";
    }

    public object ReportParcelHop(string trackingId, string code)
    {
        // Validate trackingId and code
        if (!reportTrackingIDValidator.Validate(trackingId).IsValid || !hopValidator.Validate(code).IsValid){
        return new Error(){
                StatusCode = 400,
                ErrorMessage = "The operation failed due to an error."
            }; 
        }

        // TODO: Check if parcel or hop exists
        // if(...){
        //     return new Error(){
        //         StatusCode = 404,
        //         ErrorMessage = "Parcel does not exist with this tracking ID or hop with code not found."
        //     }; 
        // }
        
        return "Successfully reported hop."; 
    }
}