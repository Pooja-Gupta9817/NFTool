using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;

using DesktopTool.App.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
          //  _httpClient.DefaultRequestHeaders.Authorization =
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


        public async Task<(bool success, string token, UserInfoDto user)> LoginAsync(string email, string password)
        {
            var user = new LoginDto
            {
                Email = email,
                Password = password,
            };

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/login", content);
            if (!response.IsSuccessStatusCode)
                return (false, null, null);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginUserDto>(responseContent);
            _jwtToken = result.Token; // store token
            return (true, result.Token, result.User);
        }


        public async Task<bool> UploadPdfAsync(string filePath, IProgress<int> progress = null)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            if (string.IsNullOrWhiteSpace(JwtToken))
            {
                Debug.WriteLine("❌ JWT Token is NULL or EMPTY before upload.");
                return false;
            }

            try
            {
                Debug.WriteLine("✅ JWT Token being used for upload:");
                Debug.WriteLine(_jwtToken);

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                content.Add(streamContent, "file", Path.GetFileName(filePath));

                if (!string.IsNullOrWhiteSpace(JwtToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                }
                else
                {
                    Debug.WriteLine("⚠️ JWT token is missing. Upload will fail.");
                }


                // Track progress (simulate it here since HttpClient doesn't give it natively)
                for (int i = 1; i <= 40; i++)
                {
                    await Task.Delay(10);
                    progress?.Report(i);
                }

                var response = await _httpClient.PostAsync("/api/UploadPdf", content);

                progress?.Report(100);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Upload failed: {ex.Message}");
                return false;
            }
        }



    }

    public class JwtResponse
    {
        public string token { get; set; }
    }
}


