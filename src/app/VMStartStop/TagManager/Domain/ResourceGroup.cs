namespace MyCo.TagManager.Domain;

public class ResourceGroup
{
    public string Name { get; }
    public SubscriptionId SubscriptionId { get; }
    public VMStartStopTagValue TagValue { get; }

    public ResourceGroup(string name, SubscriptionId subscriptionId, VMStartStopTagValue tagValue)
    {
        Name = name;
        SubscriptionId = subscriptionId;
        TagValue = tagValue;
    }
}