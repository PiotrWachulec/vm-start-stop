namespace MyCo.TagManager.Domain;

public class VirtualMachine
{
    public string Name { get; }
    public SubscriptionId SubscriptionId { get; }
    public string ResourceGroupName { get; }
    public VMStartStopTagValue TagValue { get; }

    public VirtualMachine(string name, SubscriptionId subscriptionId, string resourceGroupName, VMStartStopTagValue tagValue)
    {
        Name = name;
        SubscriptionId = subscriptionId;
        ResourceGroupName = resourceGroupName;
        TagValue = tagValue;
    }
}
