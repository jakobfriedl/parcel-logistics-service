using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.WebhookManager.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Newtonsoft.Json;
using FH.ParcelLogistics.Services.DTOs;
using System.Text;
using System.Net;

namespace FH.ParcelLogistics.WebhookManager;

public class ParcelWebhookManager : IWebhookManager
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly IParcelRepository _parcelRepository;
    private readonly ILogger<IWebhookManager> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper; 

    public ParcelWebhookManager(IWebhookRepository webhookRepository, IParcelRepository parcelRepository, ILogger<IWebhookManager> logger, HttpClient httpClient, IMapper mapper)
    {
        _webhookRepository = webhookRepository;
        _parcelRepository = parcelRepository;
        _logger = logger;
        _httpClient = httpClient;
        _mapper = mapper;
    }

    public async Task Notify(string trackingId)
    {
        var parcel = _parcelRepository.GetByTrackingId(trackingId);
        var webhooks = _webhookRepository.GetByTrackingId(trackingId);

        var payload = JsonConvert.SerializeObject(_mapper.Map<WebhookMessage>(_mapper.Map<BusinessLogic.Entities.Parcel>(parcel)));

        try {
            await Task.WhenAll(webhooks.Select(webhook => {
                var req = new HttpRequestMessage(HttpMethod.Post, webhook.Url);
                req.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                try {
                    return _httpClient.SendAsync(req); 
                } catch (Exception e) {
                    _logger.LogError(e, $"Error while notifying webhook {webhook.Url}");
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                }
            })); 
        } catch (Exception e) {
            _logger.LogError(e, "Error while notifying webhooks");
        }

        // Delete webhooks if parcel is delivered
        if (parcel.State == DataAccess.Entities.Parcel.ParcelState.Delivered){
            foreach(var webhook in webhooks){
                _webhookRepository.Delete(webhook.Id);
            }
        }
    }
}
