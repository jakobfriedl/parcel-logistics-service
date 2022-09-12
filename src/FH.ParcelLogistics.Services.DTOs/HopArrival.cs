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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using FH.ParcelLogistics.Services.Converters;

namespace FH.ParcelLogistics.Services.DTOs {
	/// <summary>
	/// 
	/// </summary>
	[DataContract]
	public partial class HopArrival {
		/// <summary>
		/// Unique CODE of the hop.
		/// </summary>
		/// <value>Unique CODE of the hop.</value>
		[Required]
		[RegularExpression("^[A-Z]{4}\\d{1,4}$")]
		[DataMember(Name = "code", EmitDefaultValue = false)]
		public string Code { get; set; }

		/// <summary>
		/// Description of the hop.
		/// </summary>
		/// <value>Description of the hop.</value>
		[Required]
		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		/// <summary>
		/// The date/time the parcel arrived at the hop.
		/// </summary>
		/// <value>The date/time the parcel arrived at the hop.</value>
		[Required]
		[DataMember(Name = "dateTime", EmitDefaultValue = false)]
		public DateTime DateTime { get; set; }
	}
}