using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.Core;

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
        var virtualMachineResource = _armClient.GetVirtualMachineResource(new ResourceIdentifier(VirtualMachineResource.CreateResourceIdentifier("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", "test-rg", "test-vm")));

        try
        {
            var vm = await virtualMachineResource.GetAsync();
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
