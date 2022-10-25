namespace FH.ParcelLogistics.DataAccess.Interfaces;

using DataAccess.Entities; 

public interface IParcelRepository
{
    Parcel Submit(Parcel parcel);
    Parcel Update(Parcel parcel);
    bool Delete(int id); 
    IEnumerable<Parcel> GetParcels(); 
    Parcel GetById(int id);
    Parcel GetByTrackingId(string trackingId);

}
