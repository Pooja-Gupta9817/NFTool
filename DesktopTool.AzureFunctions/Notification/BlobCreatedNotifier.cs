using DesktopTool.App.Core.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace DesktopTool.AzureFunctions.Notification;

public class BlobCreatedNotifier
{
    private readonly ILogger<BlobCreatedNotifier> _logger;
    private readonly ISignalRNotifier _notifier;

    public BlobCreatedNotifier(ILogger<BlobCreatedNotifier> logger , ISignalRNotifier notifier)
    {
        _logger = logger;
        _notifier = notifier;   
    }

    [Function("BlobCreatedNotifier")]
    public async Task Run(
        [BlobTrigger("pdfuploads/{name}", Connection = "AzureWebJobsStorage")] Stream blobStream,
        string name,
        FunctionContext context)
    {
        var logger = context.GetLogger("BlobCreatedNotifier");
        logger.LogInformation($"🧾 PDF uploaded: {name}");

        string message = $"📄 New PDF uploaded: {name}";
        await _notifier.NotifyFileUploadedAsync(message);
    }
}
