using MyCo.TagManager.Domain;

namespace MyCo.TagManager.Application;

public interface ITagManagerService
{
    public VMStates IsCurrentTag(VMStartStopTagValue tagValue);
    public Task GetTagsFromAzure();
}