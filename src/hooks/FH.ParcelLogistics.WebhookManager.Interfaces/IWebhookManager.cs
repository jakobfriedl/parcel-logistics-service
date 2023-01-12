namespace FH.ParcelLogistics.WebhookManager.Interfaces;
public interface IWebhookManager
{
    Task Notify(string trackingId); 
}
