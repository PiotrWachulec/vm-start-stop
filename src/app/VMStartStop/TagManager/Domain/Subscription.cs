namespace MyCo.TagManager.Domain;

public class SubscriptionId
{
    public string Id { get; }

    public SubscriptionId(string id)
    {
        Id = id;
    }
}

public class Subscription
{
    public SubscriptionId Id { get; }
    public VMStartStopTagValue TagValue { get; }

    public Subscription(SubscriptionId subscriptionId, VMStartStopTagValue tagValue)
    {
        Id = subscriptionId;
        TagValue = tagValue;
    }
}