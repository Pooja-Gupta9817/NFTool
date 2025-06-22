using App.UI.ViewModel;
using DesktopTool.App.Core;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.Core.Models;
using DesktopTool.App.UI.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private readonly IRoleBasedDashboardService _roleWindowService;
        private readonly IServiceProvider _serviceProvider;
        private bool _isDarkTheme = false;
      

        public LoginViewModel(IAuthService authService, IRoleBasedDashboardService roleWindowService , IServiceProvider serviceProvider)
        {
            _authService = authService;
            _roleWindowService= roleWindowService;
            SubmitCommand = new RelayCommand(async () => await SubmitAsync(), () => !IsBusy);
            Debug.WriteLine($"🧪 AuthService instance hash: {_authService.GetHashCode()}");

            _serviceProvider= serviceProvider;
            var theme = _isDarkTheme ? "DarkTheme.xaml" : "LightTheme.xaml";
            ThemeManager.ApplyTheme(theme);

        }
        public Action CloseAction { get; set; }

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
                    OnPropertyChanged(nameof(IsRegisterMode));
                    OnPropertyChanged(nameof(ActionButtonText));
                    Name=string.Empty;
                    Email=string.Empty;
                    ErrorMessage = string.Empty;
                }
            }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                // Notify command that CanExecute might have changed
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand ToggleThemeCommand => new RelayCommand(() =>
        {
            _isDarkTheme = !_isDarkTheme;
            ThemeManager.ApplyTheme(_isDarkTheme ? "DarkTheme.xaml" : "LightTheme.xaml");
        });

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
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand SubmitCommand { get; }


        private async Task SubmitAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                ErrorMessage = string.Empty;

                if (IsLoginMode)
                {
                    if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                    {
                        ErrorMessage = "Email and Password are required.";
                        return;
                    }

                    if (!IsValidEmail(Email))
                    {
                        ErrorMessage = "Invalid email format.";
                        return;
                    }

                    var (success, token, user) = await _authService.LoginAsync(Email, Password);
                    if (success)
                    {
                        var userContext = _serviceProvider.GetRequiredService<IUserContext>();
                        userContext.Email = user.Email;
                        userContext.Name = user.Name;
                        userContext.Role = user.Role;
                        //userContext.

                        _roleWindowService.GetDashboardForRole(user.Role);
                        CloseAction?.Invoke();
                    }
                    else
                    {
                        ErrorMessage = "Login Failed";
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                    {
                        ErrorMessage = "All fields are required.";
                        return;
                    }

                    if (!IsValidEmail(Email))
                    {
                        ErrorMessage = "Invalid email format.";
                        return;
                    }

                    var role = IsTeacher ? "Teacher" : "Student";
                    var success = await _authService.RegisterAsync(Name, Email, Password, role);
                    ErrorMessage = success ? "Registration Successful!" : "User already exists";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OpenTeacherWindow()
        {
            //var teacherWindow = new TeacherWindow(); // Create this
            //teacherWindow.Show();
            //Application.Current.Windows[0]?.Close(); // Close login
        }

        private void OpenStudentWindow()
        {
            //var studentWindow = new StudentWindow(); // Create this
            //studentWindow.Show();
            //Application.Current.Windows[0]?.Close(); // Close login
        }
    }
}
