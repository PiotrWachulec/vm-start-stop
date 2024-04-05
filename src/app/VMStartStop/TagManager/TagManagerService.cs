using Microsoft.Extensions.Logging;

namespace MyCo.TagManager
{
    public class TagManagerService : ITagManagerService
    {
        private readonly ILogger _logger;

        public TagManagerService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TagManagerService>();
        }

        bool ITagManagerService.IsCurrentTag(string tagValue)
        {
            throw new NotImplementedException();
        }
    }
}