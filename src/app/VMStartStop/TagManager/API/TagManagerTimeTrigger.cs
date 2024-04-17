using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;

namespace MyCo.TagManager.API;

public class TagManagerTimeTrigger
{
    private readonly ILogger _logger;
    private readonly ITagManagerService _tagManagerService;

    public 
    TagManagerTimeTrigger(ILoggerFactory loggerFactory, ITagManagerService tagManagerService)
    {
        _logger = loggerFactory.CreateLogger<TagManagerTimeTrigger>();
        _tagManagerService = tagManagerService;
    }

    [Function("TagManager")]
    public void Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}