using DesktopTool.App.Core;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.UI.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopTool.App.UI.ViewModel
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogoutService _logoutService;

        public string LoggedInUserName { get; }
        public string LoggedInUserEmail { get; }

        public ICommand LogoutCommand { get; }
        public ICommand OpenDashboardCommand { get; }
        public ICommand OpenMyFilesCommand { get; }
        public ICommand OpenMyStatsCommand { get; }

        public StudentViewModel(IServiceProvider serviceProvider, IUserContext userContext, ILogoutService logoutService)
        {
            _serviceProvider = serviceProvider;
            _logoutService = logoutService;

            LoggedInUserName = userContext.Name;
            LoggedInUserEmail = userContext.Email;

            OpenDashboardCommand = new RelayCommand(OpenDashboard);
            OpenMyFilesCommand = new RelayCommand(OpenMyFiles);
            OpenMyStatsCommand = new RelayCommand(OpenMyStats);
            LogoutCommand = new RelayCommand(Logout);

            OpenDashboard(); // default view
        }

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        private void OpenDashboard() =>
            CurrentViewModel = _serviceProvider.GetRequiredService<StudentDashboardViewModel>();

        private void OpenMyFiles() =>
            CurrentViewModel = _serviceProvider.GetRequiredService<MyFilesViewModel>();

        private void OpenMyStats() =>
            CurrentViewModel = _serviceProvider.GetRequiredService<StudentStatsViewModel>();

        private void Logout()
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _logoutService.LogoutAndShowLogin();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

