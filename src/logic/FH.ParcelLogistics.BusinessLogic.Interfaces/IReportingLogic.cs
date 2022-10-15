namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface IReportingLogic
{
    object ReportParcelDelivery(string trackingId);
    object ReportParcelHop(string trackingId, string code);
}
