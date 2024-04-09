using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Extensions.Logging;
using MyCo.VMStartStop;

namespace MyCo.TagManager
{
    public class TagManagerService : ITagManagerService
    {
        private readonly ILogger _logger;

        public TagManagerService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TagManagerService>();
        }

        public VMStates IsCurrentTag(VMStartStopTagValue tagValue)
        {
            if (!tagValue.IsOn)
            {
                _logger.LogInformation("Tag contains OFF, omitting VM");
                return VMStates.Omit;
            }

            throw new NotImplementedException();
        }

        public void GetTagsFromAzure()
        {
            ArmClient armClient = new ArmClient(new DefaultAzureCredential());

            var subscriptions = armClient.GetSubscriptions();
        }
    }
}