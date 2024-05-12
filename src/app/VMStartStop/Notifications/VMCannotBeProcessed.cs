using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyCo
{
    public class VMCannotBeProcessed
    {
        private readonly ILogger<VMCannotBeProcessed> _logger;

        public VMCannotBeProcessed(ILogger<VMCannotBeProcessed> logger)
        {
            _logger = logger;
        }

        [Function(nameof(VMCannotBeProcessed))]
        public async Task Run(
            [ServiceBusTrigger("turn-on-off-vm-service-bus-queue/$deadletterqueue", Connection = "ReadServiceBusConnection")]
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
