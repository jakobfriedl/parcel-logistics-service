using System.ComponentModel.DataAnnotations;

namespace FH.ParcelLogistics.DataAccess.Entities;

public class Parcel
{
    public int ParcelId { get; private set; }
    [Required]
    public float Weight { get; set; }
    [Required]
    public Recipient Recipient { get; set; }
    [Required]
    public Recipient Sender { get; set; }
    public string TrackingId { get; set; }
    
    public enum ParcelState{
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
