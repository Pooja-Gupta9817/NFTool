using App.UI.ViewModel;
using DesktopTool.App.Core;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.Core.Models;
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
        private readonly IRoleBasedDashboardService _roleWindowService;
        public LoginViewModel(IAuthService authService, IRoleBasedDashboardService roleWindowService)
        {
            _authService = authService;
            _roleWindowService= roleWindowService;
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
                    OnPropertyChanged(nameof(IsRegisterMode));
                    OnPropertyChanged(nameof(ActionButtonText));
                    Name=string.Empty;
                    Email=string.Empty;
                    ErrorMessage = string.Empty;
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
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand SubmitCommand { get; }
      

        private async Task SubmitAsync()
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
                var (success, role) = await _authService.LoginAsync(Email, Password);
                if (success)
                {
                   
                    Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)?.Close();

                    _roleWindowService.GetDashboardForRole(role);
                    

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
