namespace FH.ParcelLogistics.ServiceAgents.Interfaces;

using FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.BusinessLogic.Entities;

public interface IGeoEncodingAgent
{
    NetTopologySuite.Geometries.Point EncodeAddress(DataAccess.Entities.Recipient address);
}