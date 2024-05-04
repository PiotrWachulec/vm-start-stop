using System.Text.Json;
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
    public async Task Run(
        [ServiceBusTrigger("switch-vm-in-rg-service-bus-queue", Connection = "ReadServiceBusConnection")]
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
            ResourceGroupToSwitchData resourceGroupData = JsonSerializer.Deserialize<ResourceGroupToSwitchData>(
                message.Body.ToString(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            _logger.LogInformation("Switching VMs in resource group: {rgName}", resourceGroupData.ResourceGroupName);

            // var rg = _armClient.

            // Get the VMs in the resource group
            // Switch the VMs
            // Send the VMs to the turn-on-off-vm-service-bus-queue
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            await messageActions.DeadLetterMessageAsync(message);
            return;
        }

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}
