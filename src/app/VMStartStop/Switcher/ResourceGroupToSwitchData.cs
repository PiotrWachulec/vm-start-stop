namespace MyCo.Switcher;

public class ResourceGroupToSwitchData
{
    public required string SubscriptionId { get; init; }
    public required string ResourceGroupName { get; init; }
    public required string Action { get; init; }
}