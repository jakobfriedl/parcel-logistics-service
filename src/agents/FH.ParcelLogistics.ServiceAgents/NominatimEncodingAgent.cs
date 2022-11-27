namespace FH.ParcelLogistics.ServiceAgents;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class NominatimEncodingAgent  : IGeoEncodingAgent 
{
    public GeoCoordinate EncodeAddress(Recipient address)
    {
        var cleanedStreet = address.Street.Replace(" ", "+");
        var cleanedPostalCode = address.PostalCode.Replace("AT-", "");
        var countryRegion = address.PostalCode.Substring(0, 2);
        string URL = $"http://dev.virtualearth.net/REST/v1/Locations?countryRegion={countryRegion}&postalCode={cleanedPostalCode}&locality={address.City}&addressLine={cleanedStreet}&key=Ajt0S_IotTyCgaE_jWZauEzp7bw-l8RG4wlAQzobEioZJVEwtqD_d-y23_NhnNRF";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("User-Agent", "UAS Project GeoCoding as GeoJSON");

        //start timer
        var watch = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine("Starting timer");
        var response = client.GetStringAsync(URL).Result;

        // check if response is valid and get "coordinates" from response
        if (response != null)
        {
            var result = JsonConvert.DeserializeObject<Root>(response);
            var lat = result.resourceSets[0].resources[0].point.coordinates[0];
            var lon = result.resourceSets[0].resources[0].point.coordinates[1];
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Time elapsed: {elapsedMs} ms");


            return new GeoCoordinate { Lat = lat, Lon = lon };
        }
        else
        {
            throw new Exception($"No response for address: {address}");
        }
    }

    //Helping classes for deserializing the response
    public class Root
    {
        public string authenticationResultCode { get; set; }
        public string brandLogoUri { get; set; }
        public string copyright { get; set; }
        public List<ResourceSet> resourceSets { get; set; }
        public int statusCode { get; set; }
        public string statusDescription { get; set; }
        public string traceId { get; set; }
    }

    public class ResourceSet
    {
        public int estimatedTotal { get; set; }
        public List<Resource> resources { get; set; }
    }

    public class Resource
    {
        public string __type { get; set; }
        public List<double> bbox { get; set; }
        public string name { get; set; }
        public Point point { get; set; }
        public Address address { get; set; }
        public string confidence { get; set; }
        public string entityType { get; set; }
        public List<GeocodePoint> geocodePoints { get; set; }
        public List<string> matchCodes { get; set; }
    }

    public class Point
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Address
    {
        public string addressLine { get; set; }
        public string adminDistrict { get; set; }
        public string adminDistrict2 { get; set; }
        public string countryRegion { get; set; }
        public string formattedAddress { get; set; }
        public string locality { get; set; }
        public string postalCode { get; set; }
    }

    public class GeocodePoint
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
        public string calculationMethod { get; set; }
        public List<string> usageTypes { get; set; }
    }
}


