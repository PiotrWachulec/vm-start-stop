using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.Core;
using Azure.Messaging.ServiceBus;

namespace MyCo.Switcher;

public class TurnOnOffVm
{
    private readonly ArmClient _armClient;
    private readonly ILogger<TurnOnOffVm> _logger;

    public TurnOnOffVm(ILogger<TurnOnOffVm> logger)
    {
        _logger = logger;
        _armClient = new ArmClient(new DefaultAzureCredential());
    }

    [Function(nameof(TurnOnOffVm))]
    public async Task Run(
        [ServiceBusTrigger("turn-on-off-vm-service-bus-queue", Connection = "ReadServiceBusConnection")]
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);

        if (message.Body == null)
        {
            throw new ArgumentException("Message body is null", nameof(message));
        }

        try
        {
            VirtualMachineToSwitchData virtualMachineData = JsonSerializer.Deserialize<VirtualMachineToSwitchData>(
                message.Body.ToString(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var virtualMachineResource = _armClient.GetVirtualMachineResource(
                new ResourceIdentifier(
                    VirtualMachineResource.CreateResourceIdentifier(
                        virtualMachineData.SubscriptionId,
                        virtualMachineData.ResourceGroupName,
                        virtualMachineData.VirtualMachineName)));

            if (virtualMachineData.Action == "stop")
            {
                _logger.LogInformation("Stopping VM: {vmName}", virtualMachineData.VirtualMachineName);
                await virtualMachineResource.DeallocateAsync(Azure.WaitUntil.Completed);
                _logger.LogInformation("VM stopped: {vmName}", virtualMachineData.VirtualMachineName);
            }
            else if (virtualMachineData.Action == "start")
            {
                _logger.LogInformation("Starting VM: {vmName}", virtualMachineData.VirtualMachineName);
                await virtualMachineResource.PowerOnAsync(Azure.WaitUntil.Completed);
                _logger.LogInformation("VM started: {vmName}", virtualMachineData.VirtualMachineName);
            }
            else
            {
                throw new ArgumentException("Invalid action", nameof(virtualMachineData.Action));
            }

            await messageActions.CompleteMessageAsync(message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            await messageActions.DeadLetterMessageAsync(message);
            return;
        }
    }
}
