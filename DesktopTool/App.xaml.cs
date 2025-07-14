using DesktopTool.App.Infrastructure;
using DesktopTool.App.UI.View;
using DesktopTool.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
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
        private FileSystemWatcher _logWatcher;
        private string _logPath;
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddInfrastructure(configuration, connectionString);// ✅ Centralized DI call
                })
                .Build();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Manually launch the first view
            var loginView = _host.Services.GetRequiredService<LoginView>();
            loginView.Show();


            //  Resolve SignalRClient from DI and start
            var signalRClient = _host.Services.GetRequiredService<SignalRClient>();
            _ = signalRClient.StartAsync(); // fire-and-forget
            WatchSignalRLog();

            var builder = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
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
        private void WatchSignalRLog()
        {
            _logPath = @"C:\Users\hp\source\repos\DesktopTool\DesktopTool.AzureFunctions\bin\output\signalr.log"; // Replace with your real path

            if (!File.Exists(_logPath))
            {
                File.Create(_logPath).Dispose(); // Create file if it doesn't exist
            }

            _logWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(_logPath),
                Filter = Path.GetFileName(_logPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _logWatcher.Changed += OnLogFileChanged;
        }
        private void OnLogFileChanged(object sender, FileSystemEventArgs e)
        {
            // Invoke back to UI thread
            Dispatcher.Invoke(() =>
            {
                try
                {
                    string[] lines = File.ReadAllLines(_logPath);
                    string lastLine = lines.LastOrDefault();
                    if (!string.IsNullOrWhiteSpace(lastLine))
                    {
                        ToastService.ShowToast(lastLine); // Or MessageBox.Show for quick test
                    }
                }
                catch (IOException)
                {
                    // File may be temporarily locked, skip or retry later
                }
            });
        }

    }
}
