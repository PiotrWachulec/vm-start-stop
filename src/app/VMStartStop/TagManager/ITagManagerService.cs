using MyCo.VMStartStop;

namespace MyCo.TagManager
{
    public interface ITagManagerService
    {
        public VMStates IsCurrentTag(VMStartStopTagValue tagValue);
        public Task GetTagsFromAzure();
    }
}