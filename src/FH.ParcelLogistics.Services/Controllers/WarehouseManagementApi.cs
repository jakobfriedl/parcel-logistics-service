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

namespace FH.ParcelLogistics.Services.Controllers {
	/// <summary>
	/// 
	/// </summary>
	[ApiController]
	public class WarehouseManagementApiController : ControllerBase {
		/// <summary>
		/// Exports the hierarchy of Warehouse and Truck objects. 
		/// </summary>
		/// <response code="200">Successful response</response>
		/// <response code="400">The operation failed due to an error.</response>
		/// <response code="404">No hierarchy loaded yet.</response>
		[HttpGet]
		[Route("/warehouse")]
		[ValidateModelState]
		[SwaggerOperation("ExportWarehouses")]
		[SwaggerResponse(statusCode: 200, type: typeof(Warehouse), description: "Successful response")]
		[SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
		public virtual IActionResult ExportWarehouses() {
			//TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(200, default(Warehouse));
			//TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(400, default(Error));
			//TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(404);
			string exampleJson = null;
			exampleJson = "null";

			var example = exampleJson != null
				? JsonConvert.DeserializeObject<Warehouse>(exampleJson)
				: default(Warehouse);
			//TODO: Change the data returned
			return new ObjectResult(example);
		}

		/// <summary>
		/// Get a certain warehouse or truck by code
		/// </summary>
		/// <param name="code"></param>
		/// <response code="200">Successful response</response>
		/// <response code="400">The operation failed due to an error.</response>
		/// <response code="404">No hop with the specified id could be found.</response>
		[HttpGet]
		[Route("/warehouse/{code}")]
		[ValidateModelState]
		[SwaggerOperation("GetWarehouse")]
		[SwaggerResponse(statusCode: 200, type: typeof(Hop), description: "Successful response")]
		[SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
		public virtual IActionResult GetWarehouse([FromRoute(Name = "code")] [Required] string code) {
			//TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(200, default(Hop));
			//TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(400, default(Error));
			//TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(404);
			string exampleJson = null;
			exampleJson =
				"{\n  \"code\" : \"code\",\n  \"locationName\" : \"locationName\",\n  \"processingDelayMins\" : 0,\n  \"hopType\" : \"hopType\",\n  \"description\" : \"description\",\n  \"locationCoordinates\" : {\n    \"lon\" : 1.4658129805029452,\n    \"lat\" : 6.027456183070403\n  }\n}";

			var example = exampleJson != null
				? JsonConvert.DeserializeObject<Hop>(exampleJson)
				: default(Hop);
			//TODO: Change the data returned
			return new ObjectResult(example);
		}

		/// <summary>
		/// Imports a hierarchy of Warehouse and Truck objects. 
		/// </summary>
		/// <param name="warehouse"></param>
		/// <response code="200">Successfully loaded.</response>
		/// <response code="400">The operation failed due to an error.</response>
		[HttpPost]
		[Route("/warehouse")]
		[Consumes("application/json")]
		[ValidateModelState]
		[SwaggerOperation("ImportWarehouses")]
		[SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
		public virtual IActionResult ImportWarehouses([FromBody] Warehouse warehouse) {
			//TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(200);
			//TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
			// return StatusCode(400, default(Error));

			throw new NotImplementedException();
		}
	}
}