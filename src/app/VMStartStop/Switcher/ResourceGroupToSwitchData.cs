namespace MyCo.Switcher;

public class ResourceGroupToSwitchData
{
    private string _subscriptionId;
    private string _resourceGroupName;
    private string _action;

    public required string SubscriptionId
    {
        get
        {
            return _subscriptionId;
        }
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("SubscriptionId is required", nameof(SubscriptionId));
            }

            _subscriptionId = value;
        }
    }

    public required string ResourceGroupName
    {
        get
        {
            return _resourceGroupName;
        }
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("ResourceGroupName is required", nameof(ResourceGroupName));
            }

            _resourceGroupName = value;
        }
    }

    public required string Action
    {
        get
        {
            return _action;
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

            _action = value;
        }
    }
}