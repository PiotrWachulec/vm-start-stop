using MyCo.VMStartStop;

namespace MyCo.TagManager
{
    public interface ITagManagerService
    {
        public VMStates IsCurrentTag(string tagValue);
    }
}