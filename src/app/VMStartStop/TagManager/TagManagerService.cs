using Azure.Identity;
using Microsoft.Extensions.Logging;
using Azure.ResourceManager;
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

        public VMStates IsCurrentTag(VMStartStopTagValue tagValue)
        {
            if (!tagValue.IsOn)
            {
                _logger.LogInformation("Tag contains OFF, omitting VM");
                return VMStates.Omit;
            }

            throw new NotImplementedException();
        }

        public async Task GetTagsFromAzure()
        {
            // 1. Get all subscriptions
            // 2. Check if the subscription has the VMStartStop tag
            // - If yes, then switch all VMs on subscription
            // - If no, then:
            // 3. Get all resource groups
            // 4. Check if the resource group has the VMStartStop tag
            // - If yes, then switch all VMs on resource group
            // - If no, then:
            // 5. Get all VMs
            // 6. Check if the VM has the VMStartStop tag and switch them if needed

            ArmClient armClient = new ArmClient(new DefaultAzureCredential());

            var subscriptions = armClient.GetSubscriptions();

            var count = subscriptions.Count();

            foreach (var subscription in subscriptions)
            {
                if (subscription.Data.Tags.ContainsKey("VMStartStop"))
                {
                    _logger.LogInformation($"Subscription {subscription.Data.Id} has VMStartStop tag");
                }
                else
                {
                    _logger.LogInformation($"Subscription {subscription.Data.Id} does not have VMStartStop tag");
                }
            }

            _logger.LogInformation($"Found {count} subscriptions");
        }

        VMStates ITagManagerService.IsCurrentTag(VMStartStopTagValue tagValue)
        {
            throw new NotImplementedException();
        }
    }
}