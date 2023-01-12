using FH.ParcelLogistics.DataAccess.Entities;

namespace FH.ParcelLogistics.DataAccess.Interfaces;

public interface IWebhookRepository
{
    void Delete(long id); 
    WebhookResponse Create(WebhookResponse webhookResponse);
    IList<WebhookResponse> GetByTrackingId(string trackingId);    
}
