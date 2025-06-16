using BCrypt.Net;
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
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<User>(requestBody);

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

                // Hash password before saving
                data.PasswordHash = BCrypt.Net.BCrypt.HashPassword(data.PasswordHash);

                await _userRepository.SaveChangesAsync(data);
                await _dbContext.SaveChangesAsync();

                var successResponse = req.CreateResponse(HttpStatusCode.OK);
                await successResponse.WriteStringAsync("User registered successfully");
                return successResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Error: " + ex.Message);
                return errorResponse;
            }
        }


    }
}


