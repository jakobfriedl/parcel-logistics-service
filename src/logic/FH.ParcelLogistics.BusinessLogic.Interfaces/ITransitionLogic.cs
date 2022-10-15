using FH.ParcelLogistics.BusinessLogic.Entities;

namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface ITransitionLogic
{
    object TransitionParcel(string trackingId, Parcel parcel);
}
