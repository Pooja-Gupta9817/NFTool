using DesktopTool.App.Core.Models;
using DesktopTool.App.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace DesktopTool.AzureFunctions
{
    public class GetUserProfileFunction
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public GetUserProfileFunction(IUserRepository userRepository, ILogger<GetUserProfileFunction> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [Function("GetUserProfile")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "profile")] HttpRequestData req)
        {
            try
            {
                // Extract token from Authorization header
                if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
                    return await Unauthorized(req, "Missing Authorization header");

                var token = authHeaders.FirstOrDefault()?.Replace("Bearer ", "");
                if (string.IsNullOrWhiteSpace(token))
                    return await Unauthorized(req, "Empty token");

                // Validate JWT
                var handler = new JwtSecurityTokenHandler();
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super-secret-key")), // same as JwtTokenGenerator
                    ClockSkew = TimeSpan.Zero
                };

                ClaimsPrincipal principal = handler.ValidateToken(token, validationParams, out var validatedToken);

                // Extract email claim
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return await Unauthorized(req, "User not found");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    user.Name,
                    user.Email,
                    user.Role
                });
                return response;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                return await Unauthorized(req, "Invalid token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong.");
                var res = req.CreateResponse(HttpStatusCode.InternalServerError);
                await res.WriteStringAsync("Error retrieving profile");
                return res;
            }
        }

        private async Task<HttpResponseData> Unauthorized(HttpRequestData req, string message)
        {
            var res = req.CreateResponse(HttpStatusCode.Unauthorized);
            await res.WriteStringAsync(message);
            return res;
        }
    }
}
