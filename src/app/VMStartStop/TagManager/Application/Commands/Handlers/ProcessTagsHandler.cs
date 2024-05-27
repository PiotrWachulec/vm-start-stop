using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MyCo.TagManager.Application.Commands.Handlers;

public class ProcessTagsHandler : IRequestHandler<ProcessTags>
{
    private readonly ITagManagerService _tagManagerService;
    private readonly ILogger _logger;

    public ProcessTagsHandler(ITagManagerService tagManagerService, ILoggerFactory loggerFactory)
    {
        _tagManagerService = tagManagerService;
        _logger = loggerFactory.CreateLogger<ProcessTagsHandler>();
    }

    public Task Handle(ProcessTags request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing tags");
        _logger.LogInformation($"Time triggered: {request.TriggerTime}");
        _tagManagerService.GetTagsFromAzure(request.TriggerTime);
        return Task.CompletedTask;
    }
}