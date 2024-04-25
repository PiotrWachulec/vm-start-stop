using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application.Commands;
using System.Text.Json;

namespace MyCo.TagManager.API;

public class TagManagerTimeTrigger
{
    private readonly ILogger _logger;

    public TagManagerTimeTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TagManagerTimeTrigger>();
    }

    [Function("TagManager")]
    [ServiceBusOutput("time-trigger-service-bus-queue", Connection = "WriteServiceBusConnection")]
    public string Run([TimerTrigger("0 */15 * * * *", RunOnStartup = false)] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        _logger.LogInformation($"Log triggered at: {myTimer.ScheduleStatus.Last}");

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        return JsonSerializer.Serialize(new ProcessTags());
    }
}