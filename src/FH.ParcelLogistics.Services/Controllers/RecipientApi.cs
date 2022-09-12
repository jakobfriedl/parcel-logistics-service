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

namespace FH.ParcelLogistics.Services.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class RecipientApiController : ControllerBase
    { 
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
        public virtual IActionResult TrackParcel([FromRoute (Name = "trackingId")][Required][RegularExpression("^[A-Z0-9]{9}$")]string trackingId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(TrackingInformation));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400, default(Error));
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "{\n  \"visitedHops\" : [ {\n    \"dateTime\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"code\" : \"code\",\n    \"description\" : \"description\"\n  }, {\n    \"dateTime\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"code\" : \"code\",\n    \"description\" : \"description\"\n  } ],\n  \"futureHops\" : [ {\n    \"dateTime\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"code\" : \"code\",\n    \"description\" : \"description\"\n  }, {\n    \"dateTime\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"code\" : \"code\",\n    \"description\" : \"description\"\n  } ],\n  \"state\" : \"Pickup\"\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<TrackingInformation>(exampleJson)
            : default(TrackingInformation);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
