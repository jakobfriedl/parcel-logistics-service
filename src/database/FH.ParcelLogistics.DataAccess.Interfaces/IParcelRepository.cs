namespace FH.ParcelLogistics.DataAccess.Interfaces;

using DataAccess.Entities; 

public interface IParcelRepository
{
    Parcel Submit(Parcel parcel);
    Parcel Update(Parcel parcel);
    Parcel GetByTrackingId(string trackingId);
    bool TryGetByTrackingId(string trackingId, out Parcel parcel);
}
