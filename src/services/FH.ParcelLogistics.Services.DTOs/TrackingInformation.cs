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
	public partial class TrackingInformation {
		/// <summary>
		/// State of the parcel.
		/// </summary>
		/// <value>State of the parcel.</value>
		[TypeConverter(typeof(CustomEnumConverter<ParcelState>))]
		[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
		public enum ParcelState {
			/// <summary>
			/// Enum PickupEnum for Pickup
			/// </summary>
			[EnumMember(Value = "Pickup")] Pickup = 1,

			/// <summary>
			/// Enum InTransportEnum for InTransport
			/// </summary>
			[EnumMember(Value = "InTransport")] InTransport = 2,

			/// <summary>
			/// Enum InTruckDeliveryEnum for InTruckDelivery
			/// </summary>
			[EnumMember(Value = "InTruckDelivery")]
			InTruckDeliveryEnum = 3,

			/// <summary>
			/// Enum TransferredEnum for Transferred
			/// </summary>
			[EnumMember(Value = "Transferred")] Transferred = 4,

			/// <summary>
			/// Enum DeliveredEnum for Delivered
			/// </summary>
			[EnumMember(Value = "Delivered")] Delivered = 5
		}

		/// <summary>
		/// State of the parcel.
		/// </summary>
		/// <value>State of the parcel.</value>
		[Required]
		[DataMember(Name = "state", EmitDefaultValue = true)]
		public ParcelState State { get; set; }

		/// <summary>
		/// Hops visited in the past.
		/// </summary>
		/// <value>Hops visited in the past.</value>
		[Required]
		[DataMember(Name = "visitedHops", EmitDefaultValue = false)]
		public List<HopArrival> VisitedHops { get; set; }

		/// <summary>
		/// Hops coming up in the future - their times are estimations.
		/// </summary>
		/// <value>Hops coming up in the future - their times are estimations.</value>
		[Required]
		[DataMember(Name = "futureHops", EmitDefaultValue = false)]
		public List<HopArrival> FutureHops { get; set; }
	}
}