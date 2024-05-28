using System.Collections.ObjectModel;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Compute;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Domain;
using Microsoft.Extensions.Configuration;

namespace MyCo.TagManager.Infrastrucutre;

public class TagsRepository : ITagsRepository
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly ArmClient _armClient;
    private readonly string _vmStartStopTagKey;

    public TagsRepository(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<TagsRepository>();
        _configuration = configuration;
        _armClient = new ArmClient(new DefaultAzureCredential());

        _vmStartStopTagKey = _configuration["VM_START_STOP_TAG_NAME"];
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
            if (resourceGroup.Data.Tags.ContainsKey(_vmStartStopTagKey))
            {
                _logger.LogInformation($"Resource group {resourceGroup.Data.Id} has VMStartStop tag");
                resourceGroupWithTags.Add(
                    new ResourceGroup(
                        resourceGroup.Data.Name,
                        new SubscriptionId(subscription.Data.Id),
                        new VMStartStopTagValue(resourceGroup.Data.Tags[_vmStartStopTagKey])
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
            if (subscription.Data.Tags.ContainsKey(_vmStartStopTagKey))
            {
                _logger.LogInformation($"Subscription {subscription.Data.Id} has VMStartStop tag");
                subscriptionsWithTags.Add(
                    new Subscription(
                        new SubscriptionId(subscription.Data.Id),
                        new VMStartStopTagValue(subscription.Data.Tags[_vmStartStopTagKey])
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

    public async Task<IEnumerable<VirtualMachine>> GetTagsFromVirtualMachines()
    {
        var subscriptions = _armClient.GetSubscriptions();

        var count = subscriptions.Count();

        _logger.LogInformation($"Found {count} subscriptions");

        IEnumerable<VirtualMachine> virtualMachinesWithTags = [];

        foreach (var subscription in subscriptions)
        {
            virtualMachinesWithTags = virtualMachinesWithTags.Concat(await GetTagsFromVirtualMachinesInSubscription(subscription));
        }

        return virtualMachinesWithTags;
    }

    public async Task<IEnumerable<VirtualMachine>> GetTagsFromVirtualMachinesInSubscription(SubscriptionResource subscription)
    {
        Collection<VirtualMachine> virtualMachinesWithTags = [];

        await foreach (VirtualMachineResource virtualMachine in subscription.GetVirtualMachinesAsync())
        {
            if (virtualMachine.Data.Tags.ContainsKey(_vmStartStopTagKey))
            {
                _logger.LogInformation($"Virtual machine {virtualMachine.Data.Id} has VMStartStop tag");
                virtualMachinesWithTags.Add(
                    new VirtualMachine(
                        virtualMachine.Data.Name,
                        new SubscriptionId(subscription.Data.Id),
                        virtualMachine.Data.Id.ResourceGroupName,
                        new VMStartStopTagValue(virtualMachine.Data.Tags[_vmStartStopTagKey])
                    )
                );
            }
            else
            {
                _logger.LogInformation($"Virtual machine {virtualMachine.Data.Id} does not have VMStartStop tag");
            }
        }

        return virtualMachinesWithTags;
    }
}