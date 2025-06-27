using Azure.Storage.Blobs;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Infrastructure.Repository;
using DesktopTool.App.Infrastructure.Service;
using DesktopTool.AzureFunctions.Notification;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// ✅ Configure Kestrel (for large file uploads)
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});
Console.WriteLine($"🔧 Current Environment: {builder.Environment.EnvironmentName}");
// ✅ Register SignalR Notifier
if (builder.Environment.IsDevelopment())
{
     builder.Services.AddSingleton<ISignalRNotifier, FakeSignalRNotifier>();
    //builder.Services.AddSingleton<ISignalRNotifier, RealSignalRNotifier>();

}
else
{
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<ISignalRNotifier, RealSignalRNotifier>();
}

// ✅ Register other services
var connectionString = "Server=localhost;Database=StudentToolDb;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config["AzureWebJobsStorage"];
    return new BlobServiceClient(connStr);
});

// ✅ Start the Function host
builder.Build().Run();
