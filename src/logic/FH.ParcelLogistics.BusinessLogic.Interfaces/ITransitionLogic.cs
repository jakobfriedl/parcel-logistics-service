using FH.ParcelLogistics.BusinessLogic.Entities;

namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface ITransitionLogic
{
    Parcel TransitionParcel(string trackingId, Parcel parcel);
}
