using System.Collections.ObjectModel;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Logging;

namespace MyCo.TagManager;

public class TagsRepository : ITagsRepository
{
    private readonly ILogger _logger;
    private readonly ArmClient _armClient;

    public TagsRepository(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TagsRepository>();
        _armClient = new ArmClient(new DefaultAzureCredential());
    }

    public IEnumerable<ResourceGroup> GetTagsFromResourceGroups()
    {
        var subscriptions = _armClient.GetSubscriptions();

        var count = subscriptions.Count();

        _logger.LogInformation($"Found {count} subscriptions");

        IEnumerable<ResourceGroup> resourceGroupWithTags = [];

        foreach (var subscription in subscriptions)
        {
            resourceGroupWithTags = resourceGroupWithTags.Concat(GetTagsFromResourceGroupsInSubscription(subscription));
        }

        return resourceGroupWithTags;
    }

    public IEnumerable<ResourceGroup> GetTagsFromResourceGroupsInSubscription(SubscriptionResource subscription)
    {
        var resourceGroups = subscription.GetResourceGroups();
        
        Collection<ResourceGroup> resourceGroupWithTags = [];

        foreach (var resourceGroup in resourceGroups)
        {
            if (resourceGroup.Data.Tags.ContainsKey("VM-START-STOP-SCHEDULE"))
            {
                _logger.LogInformation($"Resource group {resourceGroup.Data.Id} has VMStartStop tag");
                resourceGroupWithTags.Add(
                    new ResourceGroup(
                        resourceGroup.Data.Name,
                        new SubscriptionId(subscription.Data.Id),
                        new VMStartStopTagValue(resourceGroup.Data.Tags["VM-START-STOP-SCHEDULE"])
                    )
                );
            }
            else
            {
                _logger.LogInformation($"Resource group {resourceGroup.Data.Id} does not have VMStartStop tag");
            }
        }

        return resourceGroupWithTags;
    }

    public IEnumerable<Subscription> GetTagsFromSubscriptions()
    {
        var subscriptions = _armClient.GetSubscriptions();

        var count = subscriptions.Count();

        _logger.LogInformation($"Found {count} subscriptions");

        Collection<Subscription> subscriptionsWithTags = [];

        foreach (var subscription in subscriptions)
        {
            if (subscription.Data.Tags.ContainsKey("VM-START-STOP-SCHEDULE"))
            {
                _logger.LogInformation($"Subscription {subscription.Data.Id} has VMStartStop tag");
                subscriptionsWithTags.Add(
                    new Subscription(
                        new SubscriptionId(subscription.Data.Id),
                        new VMStartStopTagValue(subscription.Data.Tags["VM-START-STOP-SCHEDULE"])
                    )
                );
            }
            else
            {
                _logger.LogInformation($"Subscription {subscription.Data.Id} does not have VMStartStop tag");
            }
        }

        return subscriptionsWithTags;
    }
}