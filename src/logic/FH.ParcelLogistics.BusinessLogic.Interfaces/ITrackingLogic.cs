using FH.ParcelLogistics.BusinessLogic.Entities;

namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface ITrackingLogic
{
    Parcel TrackParcel(string trackingId); 
}

