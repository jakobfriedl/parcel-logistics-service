using FH.ParcelLogistics.BusinessLogic.Entities;

namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface IWebhookLogic {
    IList<WebhookResponse> ListWebhooks(string trackingId);
    WebhookResponse Subscribe(string trackingId, string url);
    void Unsubscribe(long id);
}