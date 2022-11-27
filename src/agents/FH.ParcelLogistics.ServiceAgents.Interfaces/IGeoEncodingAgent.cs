namespace FH.ParcelLogistics.ServiceAgents.Interfaces;

using FH.ParcelLogistics.BusinessLogic.Entities;

public interface IGeoEncodingAgent
{
    //GeoCoordinate EncodeAddress(Recipient address);
    public GeoCoordinate EncodeAddress(Recipient address);
}