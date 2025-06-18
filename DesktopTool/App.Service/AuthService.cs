using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Data;
using DesktopTool.App.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;


namespace DesktopTool.App.Service 
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:7071";


        public AuthService(ApplicationDbContext context , HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
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

            var response = await _httpClient.PostAsJsonAsync("/api/register", user);
            var result = await response.Content.ReadAsStringAsync();

            MessageBox.Show(result);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> LoginAsync(string email, string password)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            //if (user == null)
            //    return false;

            var user = new
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/login", user);
            return response.IsSuccessStatusCode;

            //return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}


