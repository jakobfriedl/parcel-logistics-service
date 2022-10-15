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
using AutoMapper;
using FH.ParcelLogistics.Services.Attributes;
using FH.ParcelLogistics.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FH.ParcelLogistics.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class LogisticsPartnerApiController : ControllerBase
    {

        private readonly IMapper _mapper;
        public LogisticsPartnerApiController(IMapper mapper) { _mapper = mapper; }


        /// <summary>
        /// Transfer an existing parcel into the system from the service of a logistics partner. 
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
        /// <param name="parcel"></param>
        /// <response code="200">Successfully transitioned the parcel</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="409">A parcel with the specified trackingID is already in the system.</response>
        [HttpPost]
        [Route("/parcel/{trackingId}")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("TransitionParcel")]
        [SwaggerResponse(statusCode: 200, type: typeof(NewParcelInfo),
            description: "Successfully transitioned the parcel")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult TransitionParcel(
            [FromRoute(Name = "trackingId")][Required][RegularExpression("^[A-Z0-9]{9}$")] string trackingId,
            [FromBody] Parcel parcel)
        {
            var parcelEntity = _mapper.Map<BusinessLogic.Entities.Parcel>(parcel);
            var logic = new FH.ParcelLogistics.BusinessLogic.TransitionLogic();
            var result = logic.TransitionParcel(trackingId, parcelEntity);

            if (result is BusinessLogic.Entities.Parcel){
                return StatusCode(StatusCodes.Status200OK, _mapper.Map<NewParcelInfo>(result));
            }

            return StatusCode(StatusCodes.Status400BadRequest, _mapper.Map<DTOs.Error>(result));
        }
    }
}