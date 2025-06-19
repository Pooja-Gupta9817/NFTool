using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;
using DesktopTool.AzureFunctions.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopTool.AzureFunctions
{
    public class LoginUserFunction
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public LoginUserFunction(IUserRepository userRepository, ILogger<LoginUserFunction> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [Function("LoginUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] HttpRequestData req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                _logger.LogInformation($"Request Body Length: {requestBody?.Length}");
                var loginDto = JsonSerializer.Deserialize<LoginUserDto>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteStringAsync("Invalid email or password");

                    _logger.LogInformation($"Invalid email or password");
                    return badRequest;
                }

                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
                    await unauthorized.WriteStringAsync("Invalid credentials");

                    _logger.LogInformation($"Invalid credentials:{loginDto.Email}");
                    return unauthorized;
                }

                _logger.LogInformation($"Login Status: before token auth");

                var token = JwtTokenGenerator.GenerateToken(user.Email, user.Role);
                _logger.LogInformation("Token: " + token);
                var success = req.CreateResponse(HttpStatusCode.OK);
                await success.WriteAsJsonAsync(new { token });

                _logger.LogInformation($"Login Status: {success}");
                return success;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync("An error occurred while logging in.");
                return error;
            }
        }
    }
}
