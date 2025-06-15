using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using DesktopTool.App.Infrastructure.Service;


var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // 👇 Same as your WPF app
        services.AddInfrastructure("Server=localhost;Database=StudentToolDb;Trusted_Connection=True;TrustServerCertificate=True;");
    })
    .Build();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
