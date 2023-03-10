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
	public partial class Transferwarehouse : Hop {
		/// <summary>
		/// GeoJSON (https://geojson.org/) of the area covered by the logistics partner.
		/// </summary>
		/// <value>GeoJSON (https://geojson.org/) of the area covered by the logistics partner.</value>
		[Required]
		[DataMember(Name = "regionGeoJson", EmitDefaultValue = false)]
		public string RegionGeoJson { get; set; }

		/// <summary>
		/// Name of the logistics partner.
		/// </summary>
		/// <value>Name of the logistics partner.</value>
		[Required]
		[DataMember(Name = "logisticsPartner", EmitDefaultValue = false)]
		public string LogisticsPartner { get; set; }

		/// <summary>
		/// BaseURL of the logistics partner&#39;s REST service.
		/// </summary>
		/// <value>BaseURL of the logistics partner&#39;s REST service.</value>
		[Required]
		[DataMember(Name = "logisticsPartnerUrl", EmitDefaultValue = false)]
		public string LogisticsPartnerUrl { get; set; }
	}
}