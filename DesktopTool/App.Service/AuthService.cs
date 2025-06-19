using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Data;
using DesktopTool.App.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;



namespace DesktopTool.App.Service 
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        // private readonly string _baseUrl = "http://localhost:7071";
        private string _jwtToken;

        public string JwtToken => _jwtToken;

        public AuthService(ApplicationDbContext context , HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization =
                                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.GetToken());

        }

        public async Task<bool> RegisterAsync(string name, string email, string password, string role)
        {
            Console.WriteLine("Sending request to Azure Function");

            if (_context.Users.Any(u => u.Email == email))
                return false;

            var user = new RegisterUserDto
            {
                Name = name,
                Email = email,
                Password = password,
                Role = role
            };

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine("Sending Registration JSON: " + json); 

            var response = await _httpClient.PostAsync("/api/register", content);


            var result = await response.Content.ReadAsStringAsync();

            //string json = JsonSerializer.Serialize(user);
            //MessageBox.Show(json);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = new LoginUserDto
            {
                Email = email,
                Password = password,
            };

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/login", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JwtResponse>(responseJson);
            _jwtToken = result?.Token;
            return true;
        }

    }

    public class JwtResponse
    {
        public string Token { get; set; }
    }
}


