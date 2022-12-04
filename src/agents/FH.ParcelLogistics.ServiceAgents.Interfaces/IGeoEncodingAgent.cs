namespace FH.ParcelLogistics.ServiceAgents.Interfaces;

using FH.ParcelLogistics.BusinessLogic.Entities;

public interface IGeoEncodingAgent
{
    public GeoCoordinate EncodeAddress(Recipient address);
}