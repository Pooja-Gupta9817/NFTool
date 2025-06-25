using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DesktopTool.AzureFunctions.Functions;

public class TestHttpTrigger
{
    private readonly ILogger<TestHttpTrigger> _logger;

    public TestHttpTrigger(ILogger<TestHttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("PingTest")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString("Ping received");
        return response;
    }

}