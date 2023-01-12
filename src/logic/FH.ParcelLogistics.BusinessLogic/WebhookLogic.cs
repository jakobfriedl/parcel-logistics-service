using System.Runtime.CompilerServices;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FH.ParcelLogistics.BusinessLogic;

public class TrackingIDValidator : AbstractValidator<string>
{
    public TrackingIDValidator()
    {
        RuleFor(parcelTrackingId => parcelTrackingId).NotNull().Matches(@"^[A-Z0-9]{9}$");
    }
}

public class WebhookLogic : IWebhookLogic {
    
    private readonly IWebhookRepository _webhookRepository;
    private readonly IParcelRepository _parcelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<IWebhookLogic> _logger;
    private readonly TrackingIDValidator _trackingIdValidator = new TrackingIDValidator();

    public WebhookLogic(IWebhookRepository webhookRepository, IParcelRepository parcelRepository, IMapper mapper, ILogger<IWebhookLogic> logger) {
        _webhookRepository = webhookRepository;
        _parcelRepository = parcelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public IList<WebhookResponse> ListWebhooks(string trackingId) {
        _logger.LogDebug($"ListWebhooks: [trackingId:{trackingId}]");

        // Validate trackingId
        if(!_trackingIdValidator.Validate(trackingId).IsValid) {
            _logger.LogError($"ListWebhooks: Invalid trackingId");
            throw new BLValidationException($"The operation failed due to an error.");
        }

        try {
            var parcel = _parcelRepository.GetByTrackingId(trackingId); 
            return _mapper.Map<IList<WebhookResponse>>(_webhookRepository.GetByTrackingId(trackingId));
        } catch (DALNotFoundException e) {
            _logger.LogError($"ListWebhooks: [trackingId:{trackingId}] - No webhooks with this trackingId found.");
            throw new BLNotFoundException($"No parcel found with that tracking ID.", e);
        }
    }

    public WebhookResponse Subscribe(string trackingId, string url) {
        _logger.LogDebug($"Subscribe: [trackingId:{trackingId}] [url:{url}]");

        // Validate trackingId
        if(!_trackingIdValidator.Validate(trackingId).IsValid) {
            _logger.LogError($"Subscribe: Invalid trackingId");
            throw new BLValidationException($"The operation failed due to an error.");
        }

        try{
            var parcel = _parcelRepository.GetByTrackingId(trackingId); 
            var webhook = new WebhookResponse() {
                TrackingId = trackingId,
                CreatedAt = DateTime.Now,
                Url = url
            };
            var dalWebhook = _mapper.Map<DataAccess.Entities.WebhookResponse>(webhook);
            return _mapper.Map<WebhookResponse>(_webhookRepository.Create(dalWebhook));
        } 
        catch (DALNotFoundException e){
            _logger.LogError($"Subscribe: [trackingId:{trackingId}] [url:{url}] - No parcel with this trackingId found.");
            throw new BLNotFoundException($"No parcel found with that tracking ID.", e);
        }catch (DALException e){
            _logger.LogError($"Subscribe: [trackingId:{trackingId}] [url:{url}] - Failed to create webhook subscription.");
            throw new BLException($"The operation failed due to an error.", e);
        }
    }

    public void Unsubscribe(long id) {
        _logger.LogDebug($"Unsubscribe: [id:{id}]");

        try{
            _webhookRepository.Delete(id);
        } catch (DALNotFoundException e){
            _logger.LogError($"Unsubscribe: [id:{id}] - webhook with supplied id does not exist.");
            throw new BLNotFoundException($"Subscription does not exist.", e);
        } catch (DALException e){
            _logger.LogError($"Unsubscribe: [id:{id}] - Failed to delete webhook.");
            throw new BLException($"The operation failed due to an error.", e);
        }
    }
}