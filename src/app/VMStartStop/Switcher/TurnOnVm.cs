using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.Core;
using Azure.ResourceManager.Compute.Models;
using Azure.Messaging.ServiceBus;

namespace MyCo.Switcher;

public class TurnOnVm
{
    private readonly ArmClient _armClient;
    private readonly ILogger<TurnOnVm> _logger;

    public TurnOnVm(ILogger<TurnOnVm> logger)
    {
        _logger = logger;
        _armClient = new ArmClient(new DefaultAzureCredential());
    }

    [Function(nameof(TurnOnVm))]
    public async Task Run(
        [ServiceBusTrigger("turn-on-vm-service-bus-queue", Connection = "ReadServiceBusConnection")]
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);

        if (message.ContentType != "application/json")
        {
            throw new ArgumentException("Incorrect content type", nameof(message));
        }

        try
        {
            VirtualMachineData virtualMachineData = JsonSerializer.Deserialize<VirtualMachineData>(
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

            var vm = await virtualMachineResource.GetAsync();
            var output = await virtualMachineResource.PowerOnAsync(Azure.WaitUntil.Completed);
            _logger.LogInformation("VM started: {vmName}", virtualMachineData.VirtualMachineName);
        }
        catch (Exception e)
        {
            _logger.LogError("VM cannot be started: {vmName}", virtualMachineData.VirtualMachineName);
            _logger.LogError(e.Message);

            await messageActions.DeadLetterMessageAsync(message);
            return;
        }

        await messageActions.CompleteMessageAsync(message);
    }
}
