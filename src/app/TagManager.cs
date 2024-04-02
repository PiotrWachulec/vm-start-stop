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

        private bool IsCurrentTag(string tagValue)
        {
            // Check if the tag value is the current time to turn on or off

            return false;
        }

        [Function("TagManager")]
        public async void Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            ArmClient client = new ArmClient(new DefaultAzureCredential());

            var subscriptions = client.GetSubscriptions();

            // foreach (var subscription in subscriptions)
            // {
            //     _logger.LogInformation($"Subscription: {subscription.Data.SubscriptionId}");

            //     var resourceGroups = subscription.GetResourceGroups();

            //     foreach (var resourceGroup in resourceGroups)
            //     {
            //         _logger.LogInformation($"Resource Group: {resourceGroup.Data.Name}");

            //         var resources = resourceGroup.GetResources();

            //         foreach (var resource in resources)
            //         {
            //             _logger.LogInformation($"Resource: {resource.Data.Name}");

            //             var tags = resource.GetTags();

            //             foreach (var tag in tags)
            //             {
            //                 _logger.LogInformation($"Tag: {tag.Name}, Value: {tag.Value}");
            //             }
            //         }
            //     }
            // }
            

            // foreach (var tag in tags)
            // {
            //     _logger.LogInformation($"Tag: {tag.Name}, Value: {tag.Value}");
            // }
        }
    }
}
