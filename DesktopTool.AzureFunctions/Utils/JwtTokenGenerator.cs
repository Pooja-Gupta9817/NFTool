using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DesktopTool.AzureFunctions.Utils
{
    public static class JwtTokenGenerator
    {
        private static readonly string SecretKey = "ThisIsASecretKeyForJwtToken!ChangeIt"; // Replace with environment-based config
        private static readonly string Issuer = "DesktopTool";
        private static readonly string Audience = "DesktopToolClient";

        public static string GenerateToken(string email, string role, TimeSpan? expiration = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration ?? TimeSpan.FromHours(1)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
