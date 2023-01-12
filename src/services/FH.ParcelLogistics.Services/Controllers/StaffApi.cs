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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace FH.ParcelLogistics.Services.Controllers {
	/// <summary>
	/// 
	/// </summary>
	[ApiController]
	public class StaffApiController : ControllerBase {
		private readonly IMapper _mapper;
		private readonly IReportingLogic _reportingLogic; 
		private readonly ILogger<ControllerBase> _logger;

		public StaffApiController(IMapper mapper, IReportingLogic reportingLogic, ILogger<ControllerBase> logger) { 
			_mapper = mapper; 
			_reportingLogic = reportingLogic;
			_logger = logger;
		}

		/// <summary>
		/// Report that a Parcel has been delivered at it&#39;s final destination address. 
		/// </summary>
		/// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
		/// <response code="200">Successfully reported hop.</response>
		/// <response code="400">The operation failed due to an error.</response>
		/// <response code="404">Parcel does not exist with this tracking ID. </response>
		[HttpPost]
		[Route("/parcel/{trackingId}/reportDelivery/")]
		[ValidateModelState]
		[SwaggerOperation("ReportParcelDelivery")]
		[SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
		public virtual IActionResult ReportParcelDelivery([FromRoute(Name = "trackingId")] [Required] [RegularExpression("^[A-Z0-9]{9}$")] string trackingId) {
			try {
				_reportingLogic.ReportParcelDelivery(trackingId);
				return Ok();
			} catch(BLValidationException e){
				_logger.LogError(e, $"ReportParcelDelivery: [trackingId:{trackingId}] invalid");
				return BadRequest(new Error(){ErrorMessage = e.Message});
			} catch(BLNotFoundException e) {
				_logger.LogError(e, $"ReportParcelDelivery: [trackingId:{trackingId}] not found");
				return NotFound(new Error(){ErrorMessage = e.Message});
			} catch (BLException e) {
				_logger.LogError($"ReportParcelDelivery: [trackingId:{trackingId}] failed");
				return BadRequest(new Error(){ErrorMessage = e.Message});
			}
		}

		/// <summary>
		/// Report that a Parcel has arrived at a certain hop either Warehouse or Truck. 
		/// </summary>
		/// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
		/// <param name="code">The Code of the hop (Warehouse or Truck).</param>
		/// <response code="200">Successfully reported hop.</response>
		/// <response code="404">Parcel does not exist with this tracking ID or hop with code not found. </response>
		/// <response code="400">The operation failed due to an error.</response>
		[HttpPost]
		[Route("/parcel/{trackingId}/reportHop/{code}")]
		[ValidateModelState]
		[SwaggerOperation("ReportParcelHop")]
		[SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
		public virtual IActionResult ReportParcelHop(
			[FromRoute(Name = "trackingId")] [Required] [RegularExpression("^[A-Z0-9]{9}$")] string trackingId,
			[FromRoute(Name = "code")] [Required] [RegularExpression(@"^[A-Z]{4}\d{1,4}$")] string code) 
		{	
			try {
				_reportingLogic.ReportParcelHop(trackingId, code);
				return Ok();
			} catch(BLValidationException e){
				_logger.LogError(e, $"ReportParcelHop: [trackingId:{trackingId}] invalid");
				return BadRequest(new Error(){ErrorMessage = e.Message});
			} catch(BLNotFoundException e) {
				_logger.LogError(e, $"ReportParcelHop: [trackingId:{trackingId}] not found");
				return NotFound(new Error(){ErrorMessage = e.Message});
			} catch (BLException e) {
				_logger.LogError($"ReportParcelHop: [trackingId:{trackingId}] failed");
				return BadRequest(new Error(){ErrorMessage = e.Message});
			}
		}
    }
}