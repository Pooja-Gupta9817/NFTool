using DesktopTool.App.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DesktopTool.AzureFunctions
{
    public class RegisterUserFunction
    {

        [Function("RegisterUser")]
        public async Task<HttpResponseData> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register")] HttpRequestData req,
             FunctionContext context)
        {
            var logger = context.GetLogger("RegisterUser");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync("Empty request body");
                return errorResponse;
            }

            var user = JsonSerializer.Deserialize<User>(requestBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"User {user?.Name} registered!");
            return response;
        }
    }
}


