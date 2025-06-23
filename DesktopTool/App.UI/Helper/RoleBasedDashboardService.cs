using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.UI.View;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DesktopTool.App.Service
{
    public class RoleBasedDashboardService : IRoleBasedDashboardService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MainWindow _mainWindow;

        public RoleBasedDashboardService(IServiceProvider serviceProvider, MainWindow mainWindow)
        {
            _serviceProvider = serviceProvider;
            _mainWindow = mainWindow;
        }

        public void GetDashboardForRole(string role)
        {
            UserControl view = role switch
            {
                "Teacher" => _serviceProvider.GetRequiredService<TeacherView>(),
                "Student" => _serviceProvider.GetRequiredService<StudentView>(),
                _ => throw new InvalidOperationException("Unknown role")
            };

            _mainWindow.SetContent(view);
            _mainWindow.Show();
        }

    }
}
