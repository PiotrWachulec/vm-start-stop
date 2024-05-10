using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.Switcher;

namespace MyCo
{
    public class GetVmsFromSubToSwitch
    {
        private readonly ArmClient _armClient;
        private readonly ILogger<GetVmsFromSubToSwitch> _logger;

        public GetVmsFromSubToSwitch(ILogger<GetVmsFromSubToSwitch> logger)
        {
            _logger = logger;
            _armClient = new ArmClient(new DefaultAzureCredential());
        }

        [Function(nameof(GetVmsFromSubToSwitch))]
        [ServiceBusOutput("turn-on-off-vm-service-bus-queue", Connection = "WriteServiceBusConnection")]
        public async Task<ICollection<VirtualMachineToSwitchData>> Run(
            [ServiceBusTrigger("switch-vm-in-sub-service-bus-queue", Connection = "ReadServiceBusConnection")]
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
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                SubscriptionToSwitchData? resourceGroupData = JsonSerializer.Deserialize<SubscriptionToSwitchData>(
                    message.Body.ToString(), options);

                _logger.LogInformation("Switching VMs in subscription: {subId}", resourceGroupData?.SubscriptionId);

                var subscription = _armClient.GetSubscriptionResource(
                    SubscriptionResource.CreateResourceIdentifier(
                        new ResourceIdentifier(resourceGroupData?.SubscriptionId)));

                var vmsToSwitch = new List<VirtualMachineToSwitchData>();

                await foreach (VirtualMachineResource vm in subscription.GetVirtualMachinesAsync())
                {
                    vmsToSwitch.Add(new()
                    {
                        SubscriptionId = resourceGroupData.SubscriptionId,
                        ResourceGroupName = vm.Data.Id.ResourceGroupName,
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
}
