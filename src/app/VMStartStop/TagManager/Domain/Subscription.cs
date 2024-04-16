namespace MyCo.TagManager.Domain;

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