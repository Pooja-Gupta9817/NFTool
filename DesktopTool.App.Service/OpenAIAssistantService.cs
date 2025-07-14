using DesktopTool.App.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopTool.App.Service
{
    public class OpenAIAssistantService : IAIAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIAssistantService(HttpClient httpClient)
        {
            _httpClient = httpClient; 
        }

        public async Task<string> AskAsync(string question)
        {
            var requestBody = new
            {
                model = "mistralai/mistral-7b-instruct", // You can inject this from config later
                messages = new[]
               {
                    new { role = "system", content = "You are a helpful assistant for student learning." },
                    new { role = "user", content = question }
                }
            };


            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var content = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return content.Trim();
        }
    }
}
