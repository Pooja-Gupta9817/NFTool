using App.UI.ViewModel;
using DesktopTool.App.Core;
using DesktopTool.App.UI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopTool.App.UI.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public ICommand LoginCommand { get; }

        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }


        private void ExecuteLogin()
        {
            // TODO: Add actual login logic using service
            MessageBox.Show($"Logging in as {Username}");
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Username);
        }
    }
}
