using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;
using MyCo.TagManager.Application.Commands;

namespace MyCo.TagManager
{
    public class TagProcessor
    {
        private readonly ILogger<TagProcessor> _logger;
        private readonly ITagManagerService _tagManagerService;

        public TagProcessor(ITagManagerService tagManagerService, ILogger<TagProcessor> logger)
        {
            _tagManagerService = tagManagerService;
            _logger = logger;
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

            await _tagManagerService.GetTagsFromAzure();

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
