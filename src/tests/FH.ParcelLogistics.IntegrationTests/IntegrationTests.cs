using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FH.ParcelLogistics.Services.DTOs;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FH.ParcelLogistics.IntegrationTests;


[TestFixture,
    Category("IntegrationTests"),
    Description("Parcel Logistics Integration Tests")
]

public class IntegrationTests
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string _url = "https://haider-friedl-dev.azurewebsites.net";

    [SetUp]
    public void Setup()
    {
    }

    public async Task<HttpResponseMessage?> WarehouseManagementApi_POST_warehouse()
    {
        string warehouseData = File.ReadAllText("../../../../../../data/data-light.json");
        var response = await _httpClient.PostAsync($"{_url}/warehouse",
            new StringContent(warehouseData, System.Text.Encoding.UTF8, "application/json"));
        Assert.AreEqual(HttpStatusCode.OK, response?.StatusCode);
        return response;
    }

    public async Task<HttpResponseMessage?> SenderApi_POST_parcel()
    {
        var response = await _httpClient.PostAsync($"{_url}/parcel",
            new StringContent(@"{
                ""weight"": 12.345,
                ""recipient"": {
                ""name"": ""Phil"",
                ""street"": ""Währinger Straße 3"",
                ""postalCode"": ""A-1090"",
                ""city"": ""Wien"",
                ""country"": ""Austria""
                },
                ""sender"": {
                ""name"": ""Jockl"",
                ""street"": ""Riesenradplatz 7"",
                ""postalCode"": ""A-1020"",
                ""city"": ""Wien"",
                ""country"": ""Austria""
                }
            }", System.Text.Encoding.UTF8, "application/json"));
        System.Console.WriteLine($"Content: {await response.Content.ReadAsStringAsync()}");
        System.Console.WriteLine($"StatusCode: {response.StatusCode}");
        Assert.AreEqual(HttpStatusCode.Created, response?.StatusCode);
        return response;
    }

    public async Task<HttpResponseMessage?> RecipientApi_GET_parcel(string trackingId)
    {
        var response = await _httpClient.GetAsync($"{_url}/parcel/{trackingId}");
        System.Console.WriteLine($"Content: {await response.Content.ReadAsStringAsync()}");
        System.Console.WriteLine($"StatusCode: {response.StatusCode}");
        Assert.AreEqual(HttpStatusCode.OK, response?.StatusCode);
        return response;
    }

    public async Task<HttpResponseMessage?> StaffApi_POST_reportHop(string trackingId, string code)
    {
        System.Console.WriteLine($"Reporting Hop: {trackingId}, {code}");
        var response = await _httpClient.PostAsync($"{_url}/parcel/{trackingId}/reportHop/{code}", new StringContent(""));
        System.Console.WriteLine($"StatusCode: {response.StatusCode}");
        Assert.AreEqual(HttpStatusCode.OK, response?.StatusCode);
        return response;
    }
    public async Task<HttpResponseMessage?> StaffApi_POST_reportDelivery(string trackingId)
    {
        var response = await _httpClient.PostAsync($"{_url}/parcel/{trackingId}/reportDelivery/", new StringContent(""));
        Assert.AreEqual(HttpStatusCode.OK, response?.StatusCode);
        return response;
    }

    public async Task<HttpResponseMessage?> ParcelWebhooksApi_POST_webhooks(string trackingId, string url)
    {
        var encodedUrl = HttpUtility.UrlEncode(url);
        var response = await _httpClient.PostAsync($"{_url}/parcel/{trackingId}/webhooks?url={encodedUrl}",
            new StringContent(""));
        System.Console.WriteLine($"POST /webhooks StatusCode: {response.StatusCode}");
        Assert.AreEqual(HttpStatusCode.OK, response?.StatusCode);
        return response;
    }

    public async Task<HttpResponseMessage?> ParcelWebhooksApi_GET_webhooks(string trackingId)
    {
        var response = await _httpClient.GetAsync($"{_url}/parcel/{trackingId}/webhooks/");
        System.Console.WriteLine($"Content: {await response.Content.ReadAsStringAsync()}");
        Assert.AreEqual(HttpStatusCode.OK, response?.StatusCode);
        return response;
    }

    public async Task<HttpResponseMessage?> ParcelWebhooksApi_DELETE_webhooks(long id)
    {
        var response = await _httpClient.DeleteAsync($"{_url}/parcel/webhooks/{id}");
        Assert.AreEqual(HttpStatusCode.OK, response?.StatusCode);
        return response;
    }

    [Test]
    [Category("Integration")]
    public async Task ParcelJourney()
    {
        var firstHopCode = "WTTA056";
        var webhookUrl = "http://dev.test.dev/webhooks";

        await WarehouseManagementApi_POST_warehouse();

        var response = await SenderApi_POST_parcel();
        Assert.NotNull(response);
        var newParcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(await response.Content.ReadAsStringAsync());

        response = await RecipientApi_GET_parcel(newParcelInfo.TrackingId);
        Assert.NotNull(response);
        var trackedParcel = JsonConvert.DeserializeObject<TrackingInformation>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(trackedParcel);
        Assert.AreEqual(3, trackedParcel.FutureHops.Count);
        Assert.AreEqual(firstHopCode, trackedParcel.FutureHops[0].Code);

        await ParcelWebhooksApi_POST_webhooks(newParcelInfo.TrackingId, webhookUrl);
        response = await ParcelWebhooksApi_GET_webhooks(newParcelInfo.TrackingId);
        Assert.NotNull(response);
        var webhooksList = JsonConvert.DeserializeObject<List<WebhookResponse>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(webhooksList);
        Assert.AreEqual(1, webhooksList.Count);
        Assert.AreEqual(webhookUrl, webhooksList[0].Url);

        await StaffApi_POST_reportHop(newParcelInfo.TrackingId, firstHopCode);

        response = await RecipientApi_GET_parcel(newParcelInfo.TrackingId);
        Assert.NotNull(response);
        trackedParcel = JsonConvert.DeserializeObject<TrackingInformation>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(trackedParcel);
        Assert.AreEqual(2, trackedParcel.FutureHops.Count);
        Assert.AreEqual(1, trackedParcel.VisitedHops.Count);
        Assert.AreEqual(firstHopCode, trackedParcel.VisitedHops[0].Code);

        await StaffApi_POST_reportHop(newParcelInfo.TrackingId, trackedParcel.FutureHops[0].Code);

        response = await RecipientApi_GET_parcel(newParcelInfo.TrackingId);
        Assert.NotNull(response);
        trackedParcel = JsonConvert.DeserializeObject<TrackingInformation>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(trackedParcel);
        Assert.AreEqual(1, trackedParcel.FutureHops.Count);
        Assert.AreEqual(2, trackedParcel.VisitedHops.Count);

        await StaffApi_POST_reportHop(newParcelInfo.TrackingId, trackedParcel.FutureHops[0].Code);

        response = await RecipientApi_GET_parcel(newParcelInfo.TrackingId);
        Assert.NotNull(response);
        trackedParcel = JsonConvert.DeserializeObject<TrackingInformation>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(trackedParcel);
        Assert.AreEqual(0, trackedParcel.FutureHops.Count);
        Assert.AreEqual(3, trackedParcel.VisitedHops.Count);

        await StaffApi_POST_reportDelivery(newParcelInfo.TrackingId);

        response = await RecipientApi_GET_parcel(newParcelInfo.TrackingId);
        Assert.NotNull(response);
        trackedParcel = JsonConvert.DeserializeObject<TrackingInformation>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(trackedParcel);
        Assert.AreEqual(0, trackedParcel.FutureHops.Count);
        Assert.AreEqual(3, trackedParcel.VisitedHops.Count);

        response = await ParcelWebhooksApi_GET_webhooks(newParcelInfo.TrackingId);
        Assert.NotNull(response);
        webhooksList = JsonConvert.DeserializeObject<List<WebhookResponse>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(webhooksList);
    }
}