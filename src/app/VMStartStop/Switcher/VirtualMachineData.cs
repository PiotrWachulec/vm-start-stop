namespace MyCo.Switcher;

public class VirtualMachineData(string subscriptionId, string resourceGroupName, string virtualMachineName, string action)
{
    public string SubscriptionId { get; } = subscriptionId;
    public string ResourceGroupName { get; } = resourceGroupName;
    public string VirtualMachineName { get; } = virtualMachineName;
    public string Action { get; } = action;
}