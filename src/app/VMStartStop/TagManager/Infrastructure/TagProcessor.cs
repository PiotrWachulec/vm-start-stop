using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;
using MyCo.TagManager.Application.Commands;

namespace MyCo.TagManager
{
    public class TagProcessor
    {
        private readonly ILogger<TagProcessor> _logger;
        private readonly ITagManagerService _tagManagerService;
        private readonly ServiceBusClient _serviceBusClient;

        public TagProcessor(ITagManagerService tagManagerService, ILogger<TagProcessor> logger, IConfiguration configuration)
        {
            _tagManagerService = tagManagerService;
            _logger = logger;
            _serviceBusClient = new ServiceBusClient(configuration["WriteServiceBusConnection"]);
        }

        [Function(nameof(TagProcessor))]
        public async Task Run(
            [ServiceBusTrigger("time-trigger-service-bus-queue", Connection = "ReadServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var decodedMessage = JsonSerializer.Deserialize<ProcessTags>(message.Body.ToString());

            var objectsToSwitch = await _tagManagerService.GetTagsFromAzure(decodedMessage.TriggerTime);

            ServiceBusSender subscriptionsSender = _serviceBusClient.CreateSender("switch-vm-in-sub-service-bus-queue");
            ServiceBusSender resourceGroupsSender = _serviceBusClient.CreateSender("switch-vm-in-rg-service-bus-queue");
            ServiceBusSender vmsSender = _serviceBusClient.CreateSender("turn-on-off-vm-service-bus-queue");

            using ServiceBusMessageBatch subscriptionsMessageBatch = await subscriptionsSender.CreateMessageBatchAsync();

            foreach (var subscription in objectsToSwitch.Subscriptions)
            {
                subscriptionsMessageBatch.TryAddMessage(new ServiceBusMessage(JsonSerializer.Serialize(subscription)));
            }

            await subscriptionsSender.SendMessagesAsync(subscriptionsMessageBatch);

            using ServiceBusMessageBatch resourceGroupsMessageBatch = await resourceGroupsSender.CreateMessageBatchAsync();

            foreach (var resourceGroup in objectsToSwitch.ResourceGroups)
            {
                resourceGroupsMessageBatch.TryAddMessage(new ServiceBusMessage(JsonSerializer.Serialize(resourceGroup)));
            }

            await resourceGroupsSender.SendMessagesAsync(resourceGroupsMessageBatch);

            using ServiceBusMessageBatch vmsMessageBatch = await vmsSender.CreateMessageBatchAsync();

            foreach (var vm in objectsToSwitch.VMs)
            {
                vmsMessageBatch.TryAddMessage(new ServiceBusMessage(JsonSerializer.Serialize(vm)));
            }

            await vmsSender.SendMessagesAsync(vmsMessageBatch);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
