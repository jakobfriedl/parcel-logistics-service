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

        foreach(var webhook in webhooks){
            var req = new HttpRequestMessage(HttpMethod.Post, webhook.Url);
            req.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = _httpClient.Send(req);

            if (response.StatusCode != HttpStatusCode.OK){
                _logger.LogError($"Webhook notification failed for webhook with id: {webhook.Id}");
            }

            // Delete webhook if parcel is delivered
            if (parcel.State == DataAccess.Entities.Parcel.ParcelState.Delivered){
                _webhookRepository.Delete(webhook.Id);
            }
        }
    }
}
