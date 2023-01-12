using FH.ParcelLogistics.DataAccess.Entities;
using FH.ParcelLogistics.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace FH.ParcelLogistics.DataAccess.Sql;

public class WebhookRepository : IWebhookRepository
{
    private readonly DbContext _context;
    private readonly ILogger<IWebhookRepository> _logger;

    public WebhookRepository(DbContext context, ILogger<IWebhookRepository> logger){
        _context = context;
        _logger = logger;
    }

    public WebhookResponse Create(WebhookResponse webhookResponse)
    {
        _context.Database.EnsureCreated();

        if (webhookResponse is null){
            _logger.LogError($"Create: WebhookResponse is null");
            throw new DALException("Create: WebhookResponse is null");
        }

        _logger.LogDebug($"Create: Creating webhook for parcel with [trackingId:{webhookResponse.TrackingId}]");
        _context.WebhookResponses.Add(webhookResponse);
        _context.SaveChanges();
        _logger.LogDebug($"Create: Webhook created for parcel with [trackingId:{webhookResponse.TrackingId}], [webhookId:{webhookResponse.Id}]");
        return webhookResponse;
    }

    public void Delete(long id)
    {
        _context.Database.EnsureCreated();

        _logger.LogDebug($"Delete: Deleting webhook with [webhookId:{id}]");
        var webhookResponse = _context.WebhookResponses.SingleOrDefault(_ => _.Id == id);

        if (webhookResponse is null){
            _logger.LogError($"Delete: Webhook with [webhookId:{id}] not found");
            throw new DALNotFoundException($"Webhook with [webhookId:{id}] not found");
        }

        try{
            _context.WebhookResponses.Remove(webhookResponse);
            _context.SaveChanges();
            _logger.LogDebug($"Delete: Webhook with [webhookId:{id}] deleted");
        } catch (Exception e){
            _logger.LogError($"Delete: Error deleting webhook with [webhookId:{id}]");
            throw new DALException($"Error deleting webhook with [webhookId:{id}]", e);
        }
    }

    public IList<WebhookResponse> GetByTrackingId(string trackingId)
    {
        _context.Database.EnsureCreated(); 

        _logger.LogDebug($"GetByTrackingId: [trackingId:{trackingId}] Get webhook by trackingId");
        try {
            return _context.WebhookResponses
                .Where(_ => _.TrackingId == trackingId)
                .ToList();
        } catch (InvalidOperationException e) {
            _logger.LogError($"GetByTrackingId: [trackingId:{trackingId}] Webhook not found");
            throw new DALNotFoundException($"Webhook with trackingId {trackingId} not found", e);
        }
    }
}