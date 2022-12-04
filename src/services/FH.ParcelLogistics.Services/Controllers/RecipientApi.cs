/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.22.1
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using FH.ParcelLogistics.Services.Attributes;
using FH.ParcelLogistics.Services.DTOs;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FH.ParcelLogistics.Services.Controllers {
	/// <summary>
	/// 
	/// </summary>
	[ApiController]
	public class RecipientApiController : ControllerBase {

		private readonly IMapper _mapper;
		private readonly ITrackingLogic _trackingLogic;
		private readonly ILogger<ControllerBase> _logger;
		
		public RecipientApiController(IMapper mapper, ITrackingLogic trackingLogic, ILogger<ControllerBase> logger) { 
			_mapper = mapper;
			_trackingLogic = trackingLogic;
			_logger = logger;
		}

		/// <summary>
		/// Find the latest state of a parcel by its tracking ID. 
		/// </summary>
		/// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
		/// <response code="200">Parcel exists, here&#39;s the tracking information.</response>
		/// <response code="400">The operation failed due to an error.</response>
		/// <response code="404">Parcel does not exist with this tracking ID.</response>
		[HttpGet]
		[Route("/parcel/{trackingId}")]
		[ValidateModelState]
		[SwaggerOperation("TrackParcel")]
		[SwaggerResponse(statusCode: 200, type: typeof(TrackingInformation), description: "Parcel exists, here&#39;s the tracking information.")]
		[SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
		public virtual IActionResult TrackParcel([FromRoute(Name = "trackingId")] [Required] [RegularExpression("^[A-Z0-9]{9}$")] string trackingId) {
			try{
				var result = _trackingLogic.TrackParcel(trackingId);
				return Ok(_mapper.Map<DTOs.TrackingInformation>(result));
			} catch(BLValidationException e){
				_logger.LogError(e, $"TrackParcel: [trackingId:{trackingId}] invalid");
				return BadRequest(new Error { ErrorMessage = e.Message });
			} catch(BLNotFoundException e){
				_logger.LogError(e, $"TrackParcel: [trackingId:{trackingId}] not found");
				return NotFound(new Error { ErrorMessage = e.Message });
			}
		}
	}
}