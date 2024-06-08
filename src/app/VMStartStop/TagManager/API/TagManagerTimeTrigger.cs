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

    [Function(nameof(TagManagerTimeTrigger))]
    [ServiceBusOutput("time-trigger-service-bus-queue", Connection = "WriteServiceBusConnection")]
    public string
     Run([TimerTrigger("0 */15 * * * *", RunOnStartup = false, UseMonitor = true)] TimerInfo myTimer)
    {
        DateTime processingTime = DateTime.Now;

        _logger.LogInformation($"C# Timer trigger function executed at: {processingTime}");

        if (myTimer.IsPastDue)
        {
            _logger.LogInformation("Timer is running late!");
            return null;
        }

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        if (myTimer.ScheduleStatus is null)
        {
            throw new Exception("No info about time trigger");
        }

        return JsonSerializer.Serialize(new ProcessTags(TimeOnly.FromTimeSpan(myTimer.ScheduleStatus.Last.TimeOfDay)));
    }
}