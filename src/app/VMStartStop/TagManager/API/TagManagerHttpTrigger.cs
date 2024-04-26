using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application.Commands;

namespace MyCo.TagManager.API;

public class TagManagerHttpTrigger
{
    private readonly ILogger _logger;

    public TagManagerHttpTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TagManagerHttpTrigger>();
    }

    [Function(nameof(TagManagerHttpTrigger))]
    public OutputType Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("VM Start Stop: Welcome to Azure Functions!");

        return new OutputType()
        {
            OutputEvent = JsonSerializer.Serialize(new ProcessTags()),
            HttpResponse = response
        };
    }
}

public class OutputType
{
   [ServiceBusOutput("time-trigger-service-bus-queue", Connection = "WriteServiceBusConnection")]
   public string OutputEvent { get; set; }

   public HttpResponseData HttpResponse { get; set; }
}
