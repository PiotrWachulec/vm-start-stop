namespace MyCo.TagManager
{
    public class VirtualMachine
    {
        public string Name { get; }
        public SubscriptionId SubscriptionId { get; }
        public VMStartStopTagValue TagValue { get; }

        public VirtualMachine(string name, SubscriptionId subscriptionId, VMStartStopTagValue tagValue)
        {
            Name = name;
            SubscriptionId = subscriptionId;
            TagValue = tagValue;
        }
    }
}