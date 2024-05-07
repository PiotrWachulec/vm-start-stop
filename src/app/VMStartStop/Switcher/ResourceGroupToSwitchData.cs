namespace MyCo.Switcher;

public class ResourceGroupToSwitchData
{
    public required string SubscriptionId
    {
        get
        {
            return SubscriptionId;
        }
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("SubscriptionId is required", nameof(SubscriptionId));
            }
        }
    }

    public required string ResourceGroupName
    {
        get
        {
            return ResourceGroupName;
        }
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("ResourceGroupName is required", nameof(ResourceGroupName));
            }
        }
    }

    public required string Action
    {
        get
        {
            return Action;
        }
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Action is required", nameof(Action));
            }
            if (value != "start" && value != "stop")
            {
                throw new ArgumentException("Action must be either 'start' or 'stop'", nameof(Action));
            }
        }
    }
}