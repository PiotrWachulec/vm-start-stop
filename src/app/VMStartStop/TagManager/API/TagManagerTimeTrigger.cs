using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;
using MyCo.TagManager.Application.Commands;
using System.Text.Json;

namespace MyCo.TagManager.API;

public class TagManagerTimeTrigger
{
    private readonly ILogger _logger;
    private readonly ITagManagerService _tagManagerService;

    public TagManagerTimeTrigger(ILoggerFactory loggerFactory, ITagManagerService tagManagerService)
    {
        _logger = loggerFactory.CreateLogger<TagManagerTimeTrigger>();
        _tagManagerService = tagManagerService;
    }

    [Function("TagManager")]
    [ServiceBusOutput("time-trigger-service-bus-queue", Connection = "WriteServiceBusConnection")]
    public string Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        _logger.LogInformation($"Log triggered at: {myTimer.ScheduleStatus.Last}");

        _logger.LogInformation("Processing tags");
        _logger.LogInformation("Tags processed");

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        return JsonSerializer.Serialize(new ProcessTags());
    }
}