using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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

        _logger.LogInformation("Trigger time: {triggerTime}", req.Query["triggerTime"]);

        TimeOnly triggerTime = TimeOnly.Parse(req.Query["triggerTime"], CultureInfo.InvariantCulture);

        return new OutputType()
        {
            OutputEvent = JsonSerializer.Serialize(new ProcessTags(triggerTime)),
            HttpResponse = new OkObjectResult("VM Start Stop: Welcome to Azure Functions!")
        };
    }
}

public class OutputType
{
   [ServiceBusOutput("time-trigger-service-bus-queue", Connection = "WriteServiceBusConnection")]
   public string OutputEvent { get; set; }

   public ObjectResult HttpResponse { get; set; }
}
