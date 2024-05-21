using MediatR;

namespace MyCo.TagManager.Application.Commands;

public class ProcessTags(TimeOnly triggerTime) : IRequest
{
    public TimeOnly TriggerTime { get; } = triggerTime;
}