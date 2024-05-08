using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyCo
{
    public class GetVmsFromSubToSwitch
    {
        private readonly ILogger<GetVmsFromSubToSwitch> _logger;

        public GetVmsFromSubToSwitch(ILogger<GetVmsFromSubToSwitch> logger)
        {
            _logger = logger;
        }

        [Function(nameof(GetVmsFromSubToSwitch))]
        [ServiceBusOutput("turn-on-off-vm-service-bus-queue", Connection = "WriteServiceBusConnection")]
        public async Task Run(
            [ServiceBusTrigger("switch-vm-in-sub-service-bus-queue", Connection = "ReadServiceBusConnection")]
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
