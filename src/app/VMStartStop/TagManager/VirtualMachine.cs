namespace MyCo.TagManager
{
    public class VirtualMachine
    {
        public string Name { get; }
        public SubscriptionId SubscriptionId { get; }
        public ResourceGroup ResourceGroup { get; }
        public VMStartStopTagValue TagValue { get; }

        public VirtualMachine(string name, SubscriptionId subscriptionId, ResourceGroup resourceGroup, VMStartStopTagValue tagValue)
        {
            Name = name;
            SubscriptionId = subscriptionId;
            ResourceGroup = resourceGroup;
            TagValue = tagValue;
        }
    }
}