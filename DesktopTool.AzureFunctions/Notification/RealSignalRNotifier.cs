// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging;
using DesktopTool.App.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DesktopTool.AzureFunctions.Notification;

public class RealSignalRNotifier : ISignalRNotifier
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RealSignalRNotifier> _logger;
    private readonly string _signalRConnectionString;
    private readonly IAsyncCollector<SignalRMessage> _signalRMessages;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public RealSignalRNotifier(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<RealSignalRNotifier> logger, IAsyncCollector<SignalRMessage> signalRMessages,
                                    HttpClient httpClient)
    {
        _httpClientFactory = httpClientFactory;
        _signalRConnectionString = config["AzureSignalRConnectionString"];
        _logger = logger;
        _signalRMessages = signalRMessages;
        _httpClient = httpClient;
        _config = config;
    }
    

    public  Task NotifyFileUploadedAsync(string fileUrl)
    {
        //var signalRConnectionString = _config["AzureSignalRConnectionString"];
        //// You'll need to parse and generate auth token (or use Azure SDK helper)
        //var endpoint = $"{signalR_base_url}/api/v1/hubs/uploadHub"; // adjust path as needed

        //var payload = new
        //{
        //    target = "NotifyUpload",
        //    arguments = new[] { fileUrl }
        //};

        //var json = JsonSerializer.Serialize(payload);
        //var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        //{
        //    Content = new StringContent(json, Encoding.UTF8, "application/json")
        //};

        //// ?? Add Authorization header here if calling Azure-hosted SignalR
        //// request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", yourJwtToken);

        //var response = await _httpClient.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        //var message = $"Simulated upload: {fileUrl}";
        //File.AppendAllText("signalr.log", $"[{DateTime.Now}] FakeNotifier sending: {message}{Environment.NewLine}");
        return Task.CompletedTask;
    }

}