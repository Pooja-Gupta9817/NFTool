// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging;
using DesktopTool.App.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DesktopTool.AzureFunctions.Notification;

public class RealSignalRNotifier : ISignalRNotifier
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RealSignalRNotifier> _logger;
    private readonly string _signalRConnectionString;

    public RealSignalRNotifier(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<RealSignalRNotifier> logger)
    {
        _httpClientFactory = httpClientFactory;
        _signalRConnectionString = config["AzureSignalRConnectionString"];
        _logger = logger;
    }

    public async Task NotifyFileUploadedAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(_signalRConnectionString))
        {
            _logger.LogWarning("SignalR connection string missing.");
            return;
        }

        var (endpoint, accessToken) = ParseConnectionString(_signalRConnectionString);
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var message = new
        {
            target = "NotifyUpload",
            arguments = new[] { $"New file uploaded: {fileUrl}" }
        };

        var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{endpoint}/api/v1/hubs/uploadHub", content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"SignalR message failed: {response.StatusCode}");
        }
    }

    private (string endpoint, string accessToken) ParseConnectionString(string conn)
    {
        var parts = conn.Split(';');
        var endpoint = parts.First(p => p.StartsWith("Endpoint")).Split('=')[1];
        var accessKey = parts.First(p => p.StartsWith("AccessKey")).Split('=')[1];
        return ($"{endpoint}", accessKey);
    }
}