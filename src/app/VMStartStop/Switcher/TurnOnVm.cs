using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.Core;
using Azure.ResourceManager.Compute.Models;

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
        if (message.ContentType != "application/json")
        {
            throw new ArgumentException("Incorrect content type", nameof(message));
        }

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

        try
        {
            var vm = await virtualMachineResource.GetAsync();
            var output = await virtualMachineResource.PowerOnAsync(Azure.WaitUntil.Completed);
            _logger.LogInformation("VM started: {output}", output);
        }
        catch (System.Exception)
        {
            _logger.LogError("VM not found: {vmName}", message.Body.ToString());
            await messageActions.DeadLetterMessageAsync(message);
            return;
        }

        

        // if the VM is already running, do nothing
        // if the VM is stopped, start it
        // if the VM is deallocating, or deallocated, notify the user that the VM is in a state that cannot be started

        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}