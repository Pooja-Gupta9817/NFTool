using DesktopTool.App.Core.DTO;
using DesktopTool.App.Core.Interfaces;
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

namespace DesktopTool.AzureFunctions.Functions;

public class RefreshTokenFunction
{
    private readonly ILogger _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly ITokenService _tokenService; // your token generator logic

    public RefreshTokenFunction(ILoggerFactory loggerFactory, ApplicationDbContext dbContext, ITokenService tokenService)
    {
        _logger = loggerFactory.CreateLogger<RefreshTokenFunction>();
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    [Function("RefreshToken")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "refresh-token")] HttpRequestData req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonSerializer.Deserialize<RefreshTokenRequestDto>(requestBody);

        if (string.IsNullOrWhiteSpace(input?.RefreshToken))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Refresh token is required.");
            return badResponse;
        }

        var existingToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == input.RefreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        if (existingToken == null)
        {
            var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorized.WriteStringAsync("Invalid or expired refresh token.");
            return unauthorized;
        }

        // Revoke old token
        existingToken.IsRevoked = true;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == existingToken.Id);
        if (user == null)
        {

        }
            // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = existingToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        await _dbContext.SaveChangesAsync();

        var okResponse = req.CreateResponse(HttpStatusCode.OK);
        await okResponse.WriteAsJsonAsync(new TokenResponseDTO
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });

        return okResponse;
    }
}