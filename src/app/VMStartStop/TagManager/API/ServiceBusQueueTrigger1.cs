using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyCo.TagManager
{
    public class ServiceBusQueueTrigger1
    {
        private readonly ILogger<ServiceBusQueueTrigger1> _logger;

        public ServiceBusQueueTrigger1(ILogger<ServiceBusQueueTrigger1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ServiceBusQueueTrigger1))]
        public async Task Run(
            [ServiceBusTrigger("time-trigger-service-bus-queue", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
