namespace FH.ParcelLogistics.BusinessLogic.Entities;

public class Parcel
{
    public float Weight { get; set; }

    public Recipient Recipient { get; set; }

    public Recipient Sender { get; set; }

    public string TrackingId { get; set; }

    public enum ParcelState
    {
        Pickup = 1,

        InTransport = 2,

        InTruckDelivery = 3,

        Transferred = 4,

        Delivered = 5
    }

    public ParcelState State { get; set; }

    public List<HopArrival> VisitedHops { get; set; }

    public List<HopArrival> FutureHops { get; set; }
}
