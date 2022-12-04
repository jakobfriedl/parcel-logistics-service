namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface IReportingLogic
{
    void ReportParcelDelivery(string trackingId);
    void ReportParcelHop(string trackingId, string code);
}
