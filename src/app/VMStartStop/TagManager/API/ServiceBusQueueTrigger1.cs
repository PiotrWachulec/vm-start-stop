using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;
using MyCo.TagManager.Application.Commands;

namespace MyCo.TagManager
{
    public class ServiceBusQueueTrigger1
    {
        private readonly ILogger<ServiceBusQueueTrigger1> _logger;
        private readonly ITagManagerService _tagManagerService;

        public ServiceBusQueueTrigger1(ITagManagerService tagManagerService, ILogger<ServiceBusQueueTrigger1> logger)
        {
            _tagManagerService = tagManagerService;
            _logger = logger;
        }

        [Function(nameof(ServiceBusQueueTrigger1))]
        public async Task Run(
            [ServiceBusTrigger("time-trigger-service-bus-queue", Connection = "ReadServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var decodedMessage = JsonSerializer.Deserialize<ProcessTags>(message.Body.ToString());

            await _tagManagerService.GetTagsFromAzure();

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
