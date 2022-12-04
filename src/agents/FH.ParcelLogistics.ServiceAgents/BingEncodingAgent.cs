namespace FH.ParcelLogistics.ServiceAgents;

using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using BingMapsRESTToolkit;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.ServiceAgents.Interfaces;

public class BingEncodingAgend  : IGeoEncodingAgent 
{
    public GeoCoordinate EncodeAddress(Recipient address)
    {
        string URL = $"http://dev.virtualearth.net/REST/v1/Locations?country={address.Country}?postalCode={address.PostalCode}&locality={address.City}&addressLine={address.Street}&key=Ajt0S_IotTyCgaE_jWZauEzp7bw-l8RG4wlAQzobEioZJVEwtqD_d-y23_NhnNRF";

        using WebClient client = new();

        var response = client.DownloadString(URL);
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Response));
        using(var es = new MemoryStream(Encoding.UTF8.GetBytes(response)))
        {
            var result = (serializer.ReadObject(es) as Response);
            Location location = (Location)result.ResourceSets.First().Resources.First();
            if(location != null){
                return new GeoCoordinate
                {
                    Lat = location.Point.Coordinates[0],
                    Lon = location.Point.Coordinates[1]
                };
            }
            else
            {
                throw new Exception($"No response for address: {address}");
            }
        }
    }
}