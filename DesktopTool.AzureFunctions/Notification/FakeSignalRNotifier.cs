// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
using DesktopTool.App.Core.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DesktopTool.AzureFunctions.Notification;

public class FakeSignalRNotifier : ISignalRNotifier
{
    private readonly ILogger<FakeSignalRNotifier> _logger;
    public FakeSignalRNotifier(ILogger<FakeSignalRNotifier> logger)
    {
        _logger = logger;
    }

    public Task NotifyFileUploadedAsync(string fileUrl)
    {
        var message = $"Simulated upload: {fileUrl}";
        File.AppendAllText("signalr.log", $"[{DateTime.Now}] FakeNotifier sending: {message}{Environment.NewLine}");
        _logger.LogInformation($"[FakeNotifier] Simulated notification: {fileUrl}");
        return Task.CompletedTask;
    }
}