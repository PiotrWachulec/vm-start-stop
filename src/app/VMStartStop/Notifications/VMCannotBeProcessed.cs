using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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
            [ServiceBusTrigger("notify-service-bus-queue", Connection = "ReadServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            HttpClient httpClient = new();
            
            var contentObject = new{
                content = message.Body.ToString()
            };

            HttpContent content = JsonContent.Create(contentObject, new MediaTypeHeaderValue("application/json"));
            var url = "";

            var response = await httpClient.PostAsync(url, content);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
