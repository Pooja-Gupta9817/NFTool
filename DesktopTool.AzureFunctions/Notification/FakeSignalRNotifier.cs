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
    private static readonly object _fileLock = new();
    public FakeSignalRNotifier(ILogger<FakeSignalRNotifier> logger)
    {
        _logger = logger;
    }

    public Task NotifyFileUploadedAsync(string fileUrl)
    {
        var message = $"[{DateTime.Now}] FakeNotifier: {fileUrl}{Environment.NewLine}";

        lock (_fileLock) //  Prevents multiple threads writing at once
        {
            File.AppendAllText("signalr.log", message);
        }

        _logger.LogInformation($"[FakeNotifier] Simulated: {fileUrl}");
        return Task.CompletedTask;
    }
}