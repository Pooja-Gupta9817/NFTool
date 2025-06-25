using Azure.Messaging.EventGrid;
using DesktopTool.App.Core.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DesktopTool.AzureFunctions.Notification
{
    
    public class BlobCreatedNotifier
    {
        private readonly ILogger<BlobCreatedNotifier> _logger;
        private readonly ISignalRNotifier _notifier;

        public BlobCreatedNotifier(
            ILoggerFactory loggerFactory,
            ISignalRNotifier notifier)
        {
            _logger = loggerFactory.CreateLogger<BlobCreatedNotifier>();
            _notifier = notifier;
        }

        [Function("BlobCreatedNotifier")]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            _logger.LogInformation($"📦 Blob event received: {eventGridEvent.Subject}");

            try
            {
                // Deserialize event data (works in Isolated model)
                var json = eventGridEvent.Data.ToString();
                var data = JsonSerializer.Deserialize<BlobCreatedEventData>(json);

                var fileUrl = data?.Url ?? "unknown";

                _logger.LogInformation($"🔔 Notifying upload of: {fileUrl}");

                await _notifier.NotifyFileUploadedAsync(fileUrl);
            }
            catch (JsonException jex)
            {
                _logger.LogError(jex, "❌ Failed to deserialize EventGrid data.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Error in BlobCreatedNotifier.");
            }
        }

        public class BlobCreatedEventData
        {
            public string Url { get; set; }
        }
    }
}
