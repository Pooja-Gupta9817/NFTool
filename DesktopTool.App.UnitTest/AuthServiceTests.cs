using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;

using DesktopTool.App.Infrastructure;
using DesktopTool.App.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;


namespace DesktopTool.App.UnitTest
{
    
    public class AuthServiceTests
    {
        private ApplicationDbContext GetDbContextWithUser(string email = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb" + Guid.NewGuid())
                .Options;
            var context = new ApplicationDbContext(options);
            if (email != null)
            {
                context.Users.Add(new DesktopTool.App.Core.Models.User { Email = email });
                context.SaveChanges();
            }
            return context;
        }

        private HttpClient GetMockHttpClient(HttpResponseMessage response)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            return new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFalse_WhenEmailExists()
        {
            var context = GetDbContextWithUser("test@example.com");
            var httpClient = GetMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
            var service = new AuthService(context, httpClient);

            var result = await service.RegisterAsync("Test", "test@example.com", "pass", "User");

            Assert.False(result);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsTrue_OnSuccess()
        {
            var context = GetDbContextWithUser();
            var httpClient = GetMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
            var service = new AuthService(context, httpClient);

            var result = await service.RegisterAsync("Test", "new@example.com", "pass", "User");

            Assert.True(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccess_AndToken()
        {
            var loginUserDto = new LoginUserDto
            {
                Token = "jwt-token",
                User = new UserInfoDto { Email = "user@example.com" }
            };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(loginUserDto), Encoding.UTF8, "application/json")
            };
            var context = GetDbContextWithUser();
            var httpClient = GetMockHttpClient(response);
            var service = new AuthService(context, httpClient);

            var (success, token, user) = await service.LoginAsync("user@example.com", "pass");

            Assert.True(success);
            Assert.Equal("jwt-token", token);
            Assert.NotNull(user);
            Assert.Equal("user@example.com", user.Email);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFalse_OnFailure()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            var context = GetDbContextWithUser();
            var httpClient = GetMockHttpClient(response);
            var service = new AuthService(context, httpClient);

            var (success, token, user) = await service.LoginAsync("user@example.com", "wrongpass");

            Assert.False(success);
            Assert.Null(token);
            Assert.Null(user);
        }

        [Fact]
        public async Task UploadPdfAsync_ReturnsFalse_WhenFileDoesNotExist()
        {
            var context = GetDbContextWithUser();
            var httpClient = GetMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
            var service = new AuthService(context, httpClient);

            var result = await service.UploadPdfAsync("nonexistent.pdf");

            Assert.False(result);
        }

        [Fact]
        public async Task UploadPdfAsync_ReturnsFalse_WhenJwtTokenIsMissing()
        {
            var context = GetDbContextWithUser();
            var httpClient = GetMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
            var service = new AuthService(context, httpClient);

            // Create a temp file to pass the file existence check
            var tempFile = Path.GetTempFileName();
            try
            {
                var result = await service.UploadPdfAsync(tempFile);
                Assert.False(result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }

}
