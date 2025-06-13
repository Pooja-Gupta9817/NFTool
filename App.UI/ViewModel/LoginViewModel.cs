using App.UI.ViewModel;
using DesktopTool.App.Core;
using DesktopTool.App.UI.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopTool.App.UI.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            SubmitCommand = new RelayCommand(async () => await SubmitAsync());
        }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private bool _isLoginMode = true;
        public bool IsLoginMode
        {
            get => _isLoginMode;
            set
            {
                if (_isLoginMode != value)
                {
                    _isLoginMode = value;
                    OnPropertyChanged(nameof(IsLoginMode));
                    OnPropertyChanged(nameof(IsRegisterMode)); // Update UI bound to this too
                }
            }
        }

        private bool _isTeacher = false;
        public bool IsTeacher
        {
            get => _isTeacher;
            set
            {
                _isTeacher = value;
                OnPropertyChanged(nameof(IsTeacher));
                OnPropertyChanged(nameof(IsStudent));
            }
        }

        public bool IsStudent => !IsTeacher;


        public bool IsRegisterMode => !IsLoginMode;

        public string ActionButtonText => IsLoginMode ? "Login" : "Register";

        public ICommand SubmitCommand { get; }

        private async Task SubmitAsync()
        {
            if (IsLoginMode)
            {
                var success = await _authService.LoginAsync(Email, Password);
                if (success)
                    MessageBox.Show("Login Successful!");
                else
                    MessageBox.Show("Login Failed");
            }
            else
            {
                var role = IsTeacher ? "Teacher" : "Student";
                var success = await _authService.RegisterAsync(Name, Email, Password, role);
                if (success)
                    MessageBox.Show("Registration Successful!");
                else
                    MessageBox.Show("User already exists");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
