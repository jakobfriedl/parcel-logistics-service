namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface IReportingLogic
{
    Task ReportParcelDelivery(string trackingId);
    Task ReportParcelHop(string trackingId, string code);
}
