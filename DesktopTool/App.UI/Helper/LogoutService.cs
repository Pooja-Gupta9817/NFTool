using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.UI.View;
using DesktopTool.App.UI.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopTool.App.Service
{
    public class LogoutService : ILogoutService
    {
        private readonly IServiceProvider _serviceProvider;

        public LogoutService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void LogoutAndShowLogin()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Close current window
                var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                currentWindow?.Close();

                // Resolve the LoginViewModel from DI
                var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();

                try
                {
                    var loginView = new LoginView(loginViewModel);
                    loginView.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading login window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
    }

}
