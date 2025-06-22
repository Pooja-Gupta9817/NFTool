using DesktopTool.App.UI.View;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using DesktopTool.App.Infrastructure;

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

            base.OnStartup(e);
        }

    }
}
