using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;

namespace MyCo.TagManager.API;

public class TagManagerHttpTrigger
{
    private readonly ILogger _logger;
    private readonly ITagManagerService _tagManagerService;

    public TagManagerHttpTrigger(ILoggerFactory loggerFactory,
        ITagManagerService tagManagerService)
    {
        _logger = loggerFactory.CreateLogger<TagManagerHttpTrigger>();
        _tagManagerService = tagManagerService;
    }

    [Function("HttpExample")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        _tagManagerService.GetTagsFromAzure();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("VM Start Stop: Welcome to Azure Functions!");

        return response;
    }
}