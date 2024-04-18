using System.Net;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application.Commands;

namespace MyCo.TagManager.API;

public class TagManagerHttpTrigger
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public TagManagerHttpTrigger(ILoggerFactory loggerFactory, IMediator mediator)
    {
        _logger = loggerFactory.CreateLogger<TagManagerHttpTrigger>();
        _mediator = mediator;
    }

    [Function("HttpExample")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        _mediator.Send(new ProcessTags());

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("VM Start Stop: Welcome to Azure Functions!");

        return response;
    }
}