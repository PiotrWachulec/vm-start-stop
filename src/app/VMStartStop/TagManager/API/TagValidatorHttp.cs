using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyCo.TagManager.API;

public class TagValidatorHttp
{
    private readonly ILogger _logger;

    public TagValidatorHttp(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger<TagValidatorHttp>();
    }

    [Function("TagValidatorHttp")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);

        return response;
    }
}
