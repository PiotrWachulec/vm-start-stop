using Microsoft.Extensions.Logging;
using MyCo.VMStartStop;

namespace MyCo.TagManager
{
    public class TagManagerService : ITagManagerService
    {
        private readonly ILogger _logger;

        public TagManagerService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TagManagerService>();
        }

        public VMStates IsCurrentTag(string tagValue)
        {
            throw new NotImplementedException();
        }
    }
}