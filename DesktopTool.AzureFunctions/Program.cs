using Azure.Storage.Blobs;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Infrastructure.Repository;
using DesktopTool.App.Infrastructure.Service;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Allow large file uploads
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});
builder.ConfigureFunctionsWebApplication();

var connectionString = "Server=localhost;Database=StudentToolDb;Trusted_Connection=True;TrustServerCertificate=True;";
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddInfrastructure(connectionString);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();

        // Register BlobServiceClient 
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connStr = config["AzureWebJobsStorage"];
            return new BlobServiceClient(connStr);
        });
    })
    .Build();

host.Run();



