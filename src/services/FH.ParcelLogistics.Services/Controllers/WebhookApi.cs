/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.22.2
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;

using AutoMapper;
using FH.ParcelLogistics.Services.Attributes;
using FH.ParcelLogistics.Services.DTOs;
using FH.ParcelLogistics.BusinessLogic.Interfaces;

namespace FH.ParcelLogistics.Services.Controllers
{ 
    /// <summary>
    /// WebhookApiController
    /// </summary>
    [ApiController]
    public class WebhookApiController : ControllerBase
    {
        private readonly IWebhookLogic _webhookLogic;
        private readonly IMapper _mapper;

        public WebhookApiController(IWebhookLogic webhookLogic, IMapper mapper)
        {
            _webhookLogic = webhookLogic;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all registered subscriptions for the parcel webhook.
        /// </summary>
        /// <param name="trackingId"></param>
        /// <response code="200">List of webooks for the &#x60;trackingId&#x60;</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">No parcel found with that tracking ID.</response>
        [HttpGet]
        [Route("/parcel/{trackingId}/webhooks")]
        [ValidateModelState]
        [SwaggerOperation("ListParcelWebhooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<WebhookResponse>), description: "List of webooks for the &#x60;trackingId&#x60;")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult ListParcelWebhooks([FromRoute (Name = "trackingId")][Required][RegularExpression("^[A-Z0-9]{9}$")]string trackingId)
        {
            try {
               var webhooks = _webhookLogic.ListWebhooks(trackingId);
               return Ok(_mapper.Map<List<WebhookResponse>>(webhooks)); 
            } catch (BLValidationException e){
                return BadRequest(new Error() { ErrorMessage = e.Message});
            } catch (BLNotFoundException e){
                return NotFound(new Error() { ErrorMessage = e.Message});
            } 
        }

        /// <summary>
        /// Subscribe to a webhook notification for the specific parcel.
        /// </summary>
        /// <param name="trackingId"></param>
        /// <param name="url"></param>
        /// <response code="200">Successful response</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">No parcel found with that tracking ID.</response>
        [HttpPost]
        [Route("/parcel/{trackingId}/webhooks")]
        [ValidateModelState]
        [SwaggerOperation("SubscribeParcelWebhook")]
        [SwaggerResponse(statusCode: 200, type: typeof(WebhookResponse), description: "Successful response")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult SubscribeParcelWebhook([FromRoute (Name = "trackingId")][Required][RegularExpression("^[A-Z0-9]{9}$")]string trackingId, [FromQuery (Name = "url")][Required()]string url)
        {
            try {
                var webhook = _webhookLogic.Subscribe(trackingId, url);
                return Ok(_mapper.Map<WebhookResponse>(webhook));
            } catch (BLValidationException e){
                return BadRequest(new Error() { ErrorMessage = e.Message});
            } catch (BLNotFoundException e){
                return NotFound(new Error() { ErrorMessage = e.Message});
            } catch (BLException e){
                return BadRequest(new Error() { ErrorMessage = e.Message});
            }
        }

        /// <summary>
        /// Remove an existing webhook subscription.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Success</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">Subscription does not exist.</response>
        [HttpDelete]
        [Route("/parcel/webhooks/{id}")]
        [ValidateModelState]
        [SwaggerOperation("UnsubscribeParcelWebhook")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult UnsubscribeParcelWebhook([FromRoute (Name = "id")][Required]long id)
        {
            try {
                _webhookLogic.Unsubscribe(id);
                return Ok();
            } catch (BLNotFoundException e){
                return NotFound(new Error() { ErrorMessage = e.Message});
            } catch (BLException e){
                return BadRequest(new Error() { ErrorMessage = e.Message});
            }
        }
    }
}
