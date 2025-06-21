
using DesktopTool.App.Core.Models;
using DesktopTool.App.UI.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;




namespace DesktopTool.App.UI.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel VM)
        {
            InitializeComponent();
            VM.CloseAction = new Action(this.Close);
            DataContext = VM;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }

        

       // private readonly HttpClient _httpClient = new HttpClient();
        //private async void TestAzureFunction_ClickAsync(object sender, RoutedEventArgs e)
        //{
        //    var user = new
        //    {
        //        Id = 1,
        //        Name = "Test User",
        //        Email = "test@example.com",
        //        PasswordHash = "123456",
        //        Role = "Student"
        //    };

        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //        WriteIndented = true
        //    };

        //    string json = System.Text.Json.JsonSerializer.Serialize(user, options);


        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    using var client = new HttpClient();
        //    var response = await client.PostAsync("http://localhost:7071/api/register", content);

        //    string result = await response.Content.ReadAsStringAsync();
        //    MessageBox.Show($"Status: {(int)response.StatusCode}\nResult: {result}");
        //}

    }
    }

