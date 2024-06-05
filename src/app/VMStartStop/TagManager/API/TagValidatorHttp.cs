using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;

namespace MyCo.TagManager.API;

public class TagValidatorHttp
{
    private readonly ILogger _logger;
    private readonly IClock _clock;

    public TagValidatorHttp(ILoggerFactory logger, IClock clock)
    {
        _logger = logger.CreateLogger<TagValidatorHttp>();
        _clock = clock;
    }

    [Function("TagValidatorHttp")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);

        return response;
    }

    [Function("GetTimezonesHttp")]
    public IActionResult GetTimezones([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var timeZones = _clock.GetTimeZones();

        return new OkObjectResult(timeZones);
    }
}
