using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.ResourceManager;

namespace MyCo.TagManager
{
    public class TagManager
    {
        private readonly ILogger _logger;

        public TagManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TagManager>();
        }

        [Function("TagManager")]
        public void Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            ArmClient client = new ArmClient(new DefaultAzureCredential());

            var tags = 

            foreach (var tag in tags)
            {
                _logger.LogInformation($"Tag: {tag.Name}, Value: {tag.Value}");
            }
        }
    }
}
