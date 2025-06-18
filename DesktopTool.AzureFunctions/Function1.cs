using BCrypt.Net;
using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DesktopTool.AzureFunctions
{
    public class RegisterUserFunction
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;

        public RegisterUserFunction(IUserRepository userRepository, ApplicationDbContext dbContext, ILogger<RegisterUserFunction> logger)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _logger = logger;
        }
        [Function("RegisterUser")]
        public async Task<HttpResponseData> Run(
               
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register")] HttpRequestData req)
        {

            RegisterUserDto? data = null;
            try
            {

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                _logger.LogInformation("Reading request body...");
                _logger.LogInformation($"Request Body Length: {requestBody?.Length}");

                data = JsonSerializer.Deserialize<RegisterUserDto>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Request body: " + requestBody);

                if (data == null || string.IsNullOrWhiteSpace(data.Email))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid input");
                    return badResponse;
                }

                var existingUser = await _userRepository.GetByEmailAsync(data.Email);
                if (existingUser != null)
                {
                    var conflictResponse = req.CreateResponse(HttpStatusCode.Conflict);
                    await conflictResponse.WriteStringAsync("User already exists");
                    return conflictResponse;
                }

                // Create actual User entity
                var newUser = new User
                {
                    Name = data.Name,
                    Email = data.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(data.Password),
                    Role = data.Role
                };

                await _userRepository.AddAsync(newUser);
                await _userRepository.SaveChangesAsync();

                var successResponse = req.CreateResponse(HttpStatusCode.OK);
                await successResponse.WriteStringAsync("User registered successfully");
                return successResponse;

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Deserialization failed.");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync("Invalid JSON format: " + ex.Message);
                return errorResponse;
            }
        }


    }
}


