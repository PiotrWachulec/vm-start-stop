namespace MyCo.Switcher;

public class VirtualMachineData(string subscriptionId, string resourceGroupName, string virtualMachineName, string action)
{
    public required string SubscriptionId { get; init; } = subscriptionId;
    public required string ResourceGroupName { get; init; } = resourceGroupName;
    public required string VirtualMachineName { get; init; } = virtualMachineName;
    public required string Action { get; init; } = action;
}