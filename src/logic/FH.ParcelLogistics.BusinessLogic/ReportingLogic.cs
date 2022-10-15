using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FluentValidation;
namespace FH.ParcelLogistics.BusinessLogic;

public class ReportTrackingIDValidator : AbstractValidator<string>
{
    public ReportTrackingIDValidator()
    {
        RuleFor(parcelTrackingId => parcelTrackingId).NotEmpty().Matches(@"^[A-Z0-9]{9}$");
    }
}

public class ReportHopValidator : AbstractValidator<string>
{
    public ReportHopValidator()
    {
        RuleFor(hopArrivalCode => hopArrivalCode).Matches(@"[A-Za-z\d äöüÄÖÜß-]+");
    }
}

public class ReportingLogic : IReportingLogic
{
    public object ReportParcelDelivery(string trackingId)
    {
        return new Error()
        {
            ErrorMessage = "Not implemented",
        };
    }

    public object ReportParcelHop(string trackingId, string code)
    {
        return new Error()
        {
            ErrorMessage = "Not implemented",
        };
    }
}