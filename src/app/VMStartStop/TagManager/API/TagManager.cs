using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyCo.TagManager
{
    public class TagManager
    {
        private readonly ILogger _logger;
        private readonly ITagManagerService _tagManagerService;

        public TagManager(ILoggerFactory loggerFactory, ITagManagerService tagManagerService)
        {
            _logger = loggerFactory.CreateLogger<TagManager>();
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
}
