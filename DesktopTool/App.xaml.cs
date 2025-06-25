using DesktopTool.App.Infrastructure;
using DesktopTool.App.UI.View;
using DesktopTool.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DesktopTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App1 : Application
    {
        private IHost? _host;
        private string connectionString = "";

        protected override void OnStartup(StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddInfrastructure(connectionString); // ✅ Centralized DI call
                })
                .Build();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Manually launch the first view
            var loginView = _host.Services.GetRequiredService<LoginView>();
            loginView.Show();


            //  Resolve SignalRClient from DI and start
            var signalRClient = _host.Services.GetRequiredService<SignalRClient>();
            _ = signalRClient.StartAsync(); // fire-and-forget
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Stop SignalR connection and dispose
            var signalRClient = _host.Services.GetRequiredService<SignalRClient>();
            signalRClient.StopAsync().Wait(); // optional, clean shutdown
            signalRClient.Dispose();

            _host.Dispose(); // dispose of the host too

            base.OnExit(e);
        }


    }
}
