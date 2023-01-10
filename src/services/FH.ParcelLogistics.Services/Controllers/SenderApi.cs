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
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace FH.ParcelLogistics.Services.Controllers {
	/// <summary>
	/// 
	/// </summary>
	[ApiController]
	public class SenderApiController : ControllerBase {
		private readonly IMapper _mapper; 
		private readonly ISubmissionLogic _submissionLogic;
		private readonly ILogger<ControllerBase> _logger;

		public SenderApiController(IMapper mapper, ISubmissionLogic submissionLogic, ILogger<ControllerBase> logger) { 
			_mapper = mapper; 
			_submissionLogic = submissionLogic; 
			_logger = logger;
		}

		/// <summary>
		/// Submit a new parcel to the logistics service. 
		/// </summary>
		/// <param name="parcel"></param>
		/// <response code="201">Successfully submitted the new parcel</response>
		/// <response code="400">The operation failed due to an error.</response>
		/// <response code="404">The address of sender or receiver was not found.</response>
		[HttpPost]
		[Route("/parcel")]
		[Consumes("application/json")]
		[ValidateModelState]
		[SwaggerOperation("SubmitParcel")]
		[SwaggerResponse(statusCode: 201, type: typeof(NewParcelInfo), description: "Successfully submitted the new parcel")]
		[SwaggerResponse(statusCode: 400, type: typeof(DTOs.Error), description: "The operation failed due to an error.")]
		[SwaggerResponse(statusCode: 404, type: typeof(DTOs.Error), description: "The address of sender or receiver was not found.")]
		public virtual IActionResult SubmitParcel([FromBody] DTOs.Parcel parcel) {
			var parcelEntity = _mapper.Map<BusinessLogic.Entities.Parcel>(parcel);
			try{
				var result = _submissionLogic.SubmitParcel(parcelEntity);
				return Created("", _mapper.Map<DTOs.NewParcelInfo>(result));
			} catch(BLValidationException e){
				_logger.LogError(e, $"SubmitParcel: Parcel invalid");
				return BadRequest(new Error { ErrorMessage = e.Message });
			} catch(BLNotFoundException e){
				_logger.LogError(e, $"SubmitParcel: Address of sender/receiver not found");
				return NotFound(new Error { ErrorMessage = e.Message });
			} catch(BLException e){
				_logger.LogError(e, $"SubmitParcel: Error");
				return BadRequest(new Error { ErrorMessage = e.Message });
			}
		}
	}
}