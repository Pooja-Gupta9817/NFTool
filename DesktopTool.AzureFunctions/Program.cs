using DesktopTool.App.Core.Models;
using DesktopTool.App.Infrastructure;
using DesktopTool.App.Infrastructure.Service;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var connectionString = "Server=localhost;Database=StudentToolDb;Trusted_Connection=True;TrustServerCertificate=True;";
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddInfrastructure(connectionString);
        services.AddScoped<IUserRepository, UserRepository>();
    })
    .Build();

host.Run();



