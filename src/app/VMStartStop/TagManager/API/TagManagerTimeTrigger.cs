using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;
using MediatR;
using MyCo.TagManager.Application.Commands;

namespace MyCo.TagManager.API;

public class TagManagerTimeTrigger
{
    private readonly ILogger _logger;
    private readonly ITagManagerService _tagManagerService;
    
    private readonly IMediator _mediator;

    public 
    TagManagerTimeTrigger(ILoggerFactory loggerFactory, ITagManagerService tagManagerService,
        IMediator mediator)
    {
        _logger = loggerFactory.CreateLogger<TagManagerTimeTrigger>();
        _tagManagerService = tagManagerService;
        _mediator = mediator;
    }

    [Function("TagManager")]
    public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        _logger.LogInformation($"Log triggered at: {myTimer.ScheduleStatus.Last}");

        _logger.LogInformation("Processing tags");
        _mediator.Send(new ProcessTags());
        _logger.LogInformation("Tags processed");

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}