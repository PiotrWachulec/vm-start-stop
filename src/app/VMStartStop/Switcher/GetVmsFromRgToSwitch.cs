using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.Switcher;

namespace MyCo;

public class GetVmsFromRgToSwitch
{
    private readonly ArmClient _armClient;
    private readonly ILogger<GetVmsFromRgToSwitch> _logger;

    public GetVmsFromRgToSwitch(ILogger<GetVmsFromRgToSwitch> logger)
    {
        _logger = logger;
        _armClient = new ArmClient(new DefaultAzureCredential());
    }

    [Function(nameof(GetVmsFromRgToSwitch))]
    [ServiceBusOutput("turn-on-off-vm-service-bus-queue", Connection = "WriteServiceBusConnection")]
    public async Task<ICollection<VirtualMachineToSwitchData>> Run(
        [ServiceBusTrigger("switch-vm-in-rg-service-bus-queue", Connection = "ReadServiceBusConnection")]
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);

        if (message.ContentType != "application/json")
        {
            throw new ArgumentException("Incorrect content type", nameof(message));
        }

        if (message.Body == null)
        {
            throw new ArgumentException("Message body is null", nameof(message));
        }

        try
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ResourceGroupToSwitchData? resourceGroupData = JsonSerializer.Deserialize<ResourceGroupToSwitchData>(
                message.Body.ToString(), options);

            _logger.LogInformation("Switching VMs in resource group: {rgName}", resourceGroupData?.ResourceGroupName);

            var subscription = _armClient.GetSubscriptionResource(
                SubscriptionResource.CreateResourceIdentifier(
                    new ResourceIdentifier(resourceGroupData?.SubscriptionId)));

            var rgFilter = "resourceGroup eq '" + resourceGroupData?.ResourceGroupName + "'";

            _logger.LogInformation("Filtering resource group: {rgFilter}", rgFilter);

            var vms = subscription.GetVirtualMachinesAsync(filter: rgFilter);

            var vmsToSwitch = new List<VirtualMachineToSwitchData>();

            await foreach (VirtualMachineResource vm in subscription.GetVirtualMachinesAsync())
            {
                vmsToSwitch.Add(new()
                {
                    SubscriptionId = resourceGroupData.SubscriptionId,
                    ResourceGroupName = resourceGroupData.ResourceGroupName,
                    VirtualMachineName = vm.Data.Name,
                    Action = resourceGroupData.Action
                });
            }

            return vmsToSwitch;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            await messageActions.DeadLetterMessageAsync(message);

            return [];
        }
        finally
        {
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
