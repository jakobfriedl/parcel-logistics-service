namespace FH.ParcelLogistics.DataAccess.Entities;

public class WebhookResponse
{
    public long Id { get; set; }

    public string TrackingId { get; set; }

    public string Url { get; set; }

    public DateTime CreatedAt { get; set; }
}
