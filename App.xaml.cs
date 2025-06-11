using DesktopTool.App.UI.View;
using DesktopTool.App.UI.ViewModel;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = new MainWindow
            {
                Content = ServiceProvider.GetRequiredService<LoginView>()
            };
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register ViewModels
            services.AddSingleton<LoginViewModel>();

            // Register Views
            services.AddSingleton<LoginView>();
        }
    }

}
