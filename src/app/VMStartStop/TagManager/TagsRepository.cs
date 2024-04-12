using System.Collections.ObjectModel;
using Azure.Identity;
using Azure.ResourceManager;
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

    public async Task<IEnumerable<Subscription>> GetTagsFromSubscriptions()
    {
        var subscriptions = _armClient.GetSubscriptions();

        var count = subscriptions.Count();

        _logger.LogInformation($"Found {count} subscriptions");

        Collection<Subscription> subscriptionsWithTags = new Collection<Subscription>();

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