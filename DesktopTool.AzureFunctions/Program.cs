using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using DesktopTool.App.Infrastructure.Service;


var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var connectionString = "Server=localhost;Database=StudentToolDb;Trusted_Connection=True;TrustServerCertificate=True;";
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddInfrastructure(connectionString);
    })
    .Build();

host.Run();



builder.Build().Run();
