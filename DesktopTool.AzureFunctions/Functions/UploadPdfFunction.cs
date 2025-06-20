using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

namespace DesktopTool.AzureFunctions.Functions
{
    public class UploadPdfFunction
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger _logger;
        private readonly string _connectionString;

        public UploadPdfFunction(BlobServiceClient blobServiceClient, ILogger<UploadPdfFunction> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
            _connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        }

        [Function("UploadPdf")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadPdf")] HttpRequestData req)
        {
            try
            {
                var jwt = req.Headers.TryGetValues("Authorization", out var authHeaders)
                          ? authHeaders.FirstOrDefault()?.Replace("Bearer ", "")
                          : null;

                _logger.LogInformation("Token: " + jwt);

                var username = ExtractUsernameFromJwt(jwt);
                if (username == null)
                {
                    var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
                    await unauthorized.WriteStringAsync("Invalid JWT token.");
                    return unauthorized;
                }

                var contentType = req.Headers.GetValues("Content-Type").FirstOrDefault();
                var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(contentType).Boundary).Value;
                var reader = new MultipartReader(boundary, req.Body);
                var section = await reader.ReadNextSectionAsync();

                if (section == null)
                {
                    var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                    await bad.WriteStringAsync("No file section found.");
                    return bad;
                }

                var uniqueFileName = $"upload_{Guid.NewGuid()}.pdf";
                var containerClient = _blobServiceClient.GetBlobContainerClient("pdfuploads");
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(uniqueFileName);
                await blobClient.UploadAsync(section.Body, overwrite: true);

                var fileSize = section.Body.Length;

                await EnsureTableExistsAsync();
                await LogMetadataAsync(username, uniqueFileName, fileSize);

                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteStringAsync($"File uploaded: {uniqueFileName}");
                return ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload failed");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync("Upload failed");
                return error;
            }
        }

        private string? ExtractUsernameFromJwt(string? jwt)
        {
            if (string.IsNullOrEmpty(jwt)) return null;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            // Customize this to match your claim name (e.g., "preferred_username", "email", "sub", etc.)
            return token.Claims.FirstOrDefault(c => c.Type == "preferred_username" || c.Type == "upn" || c.Type == "email")?.Value;
        }

        private async Task EnsureTableExistsAsync()
        {
            var query = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PdfUploads' AND xtype='U')
                CREATE TABLE PdfUploads (
                    Id INT IDENTITY PRIMARY KEY,
                    Username NVARCHAR(255),
                    FileName NVARCHAR(255),
                    FileSize BIGINT,
                    UploadedAt DATETIME DEFAULT GETDATE()
                );
            ";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task LogMetadataAsync(string username, string fileName, long fileSize)
        {
            var insert = @"
                INSERT INTO PdfUploads (Username, FileName, FileSize)
                VALUES (@Username, @FileName, @FileSize);
            ";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(insert, conn);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@FileSize", fileSize);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
