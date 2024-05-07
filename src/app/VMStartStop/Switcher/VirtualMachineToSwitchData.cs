namespace MyCo.Switcher;

public class VirtualMachineToSwitchData
{
    public required string SubscriptionId { get; init; }
    public required string ResourceGroupName { get; init; }
    public required string VirtualMachineName { get; init; }
    public required string Action { get; init; }
}