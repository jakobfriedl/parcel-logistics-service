namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

using FH.ParcelLogistics.BusinessLogic.Entities;

public interface ISubmissionLogic
{
    Parcel SubmitParcel(Parcel parcel);
}
