using Azure.Storage.Blobs;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Infrastructure;
using DesktopTool.App.Infrastructure.Repository;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Polly;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace DesktopTool.AzureFunctions.Functions
{
    public class UploadPdfFunction
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger _logger;
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly ISignalRNotifier _notifier;

        public UploadPdfFunction(BlobServiceClient blobServiceClient, ILogger<UploadPdfFunction> logger, IConfiguration config , 
            IUploadedFileRepository uploadedFileRepository , ISignalRNotifier notifier)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
            _connectionString = config["SqlConnectionString"] ?? throw new ArgumentNullException("SqlConnectionString");
            _containerName = config["BlobContainerName"] ?? "pdfuploads";
            _uploadedFileRepository = uploadedFileRepository;
            _notifier = notifier;
        }

        [Function("UploadPdf")]
 
        public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadPdf")] HttpRequestData req,
    ClaimsPrincipal principal)
        {
            try
            {
                var token = req.Headers.FirstOrDefault(h => h.Key == "Authorization").Value?.FirstOrDefault();


                if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer "))
                    return await Unauthorized(req, "Missing or invalid token.");

                token = token.Substring("Bearer ".Length);

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var exp = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                _logger.LogInformation("exp: {exp}");

                var expiry = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
                if (expiry < DateTime.UtcNow)
                {
                    _logger.LogWarning("🚫 Token expired.");
                    return await Unauthorized(req, "Token expired.");
                }

               
                if (exp == null || DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)) < DateTimeOffset.UtcNow)
                    return await Unauthorized(req, "Token expired.");

                _logger.LogInformation("Starting upload");

                string username = ExtractUsernameFromJwt(jwt);



                if (!req.Headers.TryGetValues("Content-Type", out var contentTypeValues))
                    return await CreateBadRequest(req, "Missing Content-Type header.");

                var contentType = contentTypeValues.FirstOrDefault();
                var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(contentType).Boundary).Value;

                if (string.IsNullOrEmpty(boundary))
                    return await CreateBadRequest(req, "Boundary not found.");

                var reader = new MultipartReader(boundary, req.Body);
                var section = await reader.ReadNextSectionAsync();
                if (section == null)
                    return await CreateBadRequest(req, "No section in body.");

                var fileName = GetFileNameFromContentDisposition(section);
                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";

                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();

                var memoryStream = new MemoryStream();
                await section.Body.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // rewind to start

                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                _logger.LogInformation($"Uploading file: {fileName} with size: {memoryStream.Length} bytes");

                await blobClient.UploadAsync(memoryStream, overwrite: true);
                await LogMetadataToSqlAsync(uniqueFileName, memoryStream.Length, username);


                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteStringAsync($"File uploaded: {uniqueFileName}");

                // ✅ Add this line temporarily
                //await _notifier.NotifyFileUploadedAsync("📢 Manual Test File");
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

        private async Task<HttpResponseData> Unauthorized(HttpRequestData req, string message)
        {
            var response = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
            await response.WriteStringAsync(message);
            return response;
        }

        private string GetFileNameFromContentDisposition(MultipartSection section)
        {
            var contentDisposition = section.ContentDisposition;

            if (ContentDispositionHeaderValue.TryParse(contentDisposition, out var disposition))
            {
                var fileName = disposition.FileName.Value ?? disposition.FileNameStar.Value;
                return fileName?.Trim('"') ?? string.Empty;
            }

            return string.Empty;
        }

        private string ExtractUsernameFromJwt(JwtSecurityToken token)
        {
            try
            {
                foreach (var claim in token.Claims)
                {
                    _logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
                }

                return token.Claims.FirstOrDefault(c =>
                    c.Type == "name" || c.Type == ClaimTypes.Name)?.Value ?? "anonymous";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract username from JWT.");
                return "anonymous";
            }
        }



        private async Task LogMetadataToSqlAsync(string fileName, long size, string username)
        {
            var file = new UploadedFiles
            {
                FileName = fileName,
                Size = size,
                UploadedBy = username
            };

            await _uploadedFileRepository.AddAsync(file);
            await _uploadedFileRepository.SaveChangesAsync();
        }



        private async Task<HttpResponseData> CreateBadRequest(HttpRequestData req, string message)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync(message);
            return response;
        }
    }
}
