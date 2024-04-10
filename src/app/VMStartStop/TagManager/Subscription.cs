namespace MyCo.TagManager;

internal class SubscriptionId
{
    public string Id { get; }

    public SubscriptionId(string id)
    {
        Id = id;
    }
}

public class Subscription
{
    public readonly SubscriptionId { get; }
    public readonly VMStartStopTagValue { get; }

    public Subscription(SubscriptionId subscriptionId, VMStartStopTagValue tagValue)
    {
        SubscriptionId = subscriptionId;
        VMStartStopTagValue = tagValue;
    }
}