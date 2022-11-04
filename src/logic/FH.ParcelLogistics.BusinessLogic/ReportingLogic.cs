namespace FH.ParcelLogistics.BusinessLogic;

using System.Data;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;

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
    private readonly ReportTrackingIDValidator _reportTrackingIDValidator = new ReportTrackingIDValidator();
    private readonly ReportHopValidator _hopValidator = new ReportHopValidator();
    private readonly IParcelRepository _parcelRepository; 
    private readonly IMapper _mapper;
    
    public ReportingLogic(IMapper mapper){
        _parcelRepository = new ParcelRepository(new DbContext());
        _mapper = mapper;
    }
    public ReportingLogic(IParcelRepository parcelRepository, IMapper mapper){
        _parcelRepository = parcelRepository;
        _mapper = mapper;
    }

    public object ReportParcelDelivery(string trackingId)
    {
        // Validate trackingId
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid){
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
        if (!_reportTrackingIDValidator.Validate(trackingId).IsValid || !_hopValidator.Validate(code).IsValid){
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