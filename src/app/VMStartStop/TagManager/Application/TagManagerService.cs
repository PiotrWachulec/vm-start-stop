using Microsoft.Extensions.Logging;
using MyCo.TagManager.Domain;

namespace MyCo.TagManager.Application;

public class TagManagerService : ITagManagerService
{
    private readonly ILogger _logger;
    private readonly ITagsRepository _tagsRepository;
    private readonly IClock _clock;

    public TagManagerService(ILoggerFactory loggerFactory, ITagsRepository tagsRepository, IClock clock)
    {
        _logger = loggerFactory.CreateLogger<TagManagerService>();
        _tagsRepository = tagsRepository;
        _clock = clock;
    }

    public VMStates IsCurrentTag(VMStartStopTagValue tagValue, TimeOnly currentTime)
    {
        if (!tagValue.IsOn)
        {
            _logger.LogInformation("Tag contains OFF, omitting VM");
            return VMStates.Omit;
        }

        var localCurrentTime = _clock.ConvertUtcToLocalTime(currentTime, tagValue.TimeZone);

        if (tagValue.StartTime.CompareTo(tagValue.EndTime) < 0)
        {
            switch (localCurrentTime)
            {
                case var _ when localCurrentTime < tagValue.StartTime || localCurrentTime > tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, but it's not time to start VM");
                    return VMStates.Stopped;

                case var _ when localCurrentTime == tagValue.StartTime:
                    _logger.LogInformation("Tag contains ON, VM is starting");
                    return VMStates.TurningOn;

                case var _ when localCurrentTime > tagValue.StartTime && localCurrentTime < tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, VM is running");
                    return VMStates.Running;

                case var _ when localCurrentTime == tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, VM is stopping");
                    return VMStates.TurningOff;
            }
        }
        else if (tagValue.StartTime.CompareTo(tagValue.EndTime) > 0)
        {
            switch (localCurrentTime)
            {
                case var _ when localCurrentTime == tagValue.StartTime:
                    _logger.LogInformation("Tag contains ON, VM is starting");
                    return VMStates.TurningOn;

                case var _ when localCurrentTime == tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, VM is stopping");
                    return VMStates.TurningOff;

                case var _ when localCurrentTime > tagValue.StartTime || localCurrentTime < tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, but it's not time to stop VM");
                    return VMStates.Running;

                case var _ when localCurrentTime > tagValue.EndTime && localCurrentTime < tagValue.StartTime:
                    _logger.LogInformation("Tag contains ON, VM is stopped");
                    return VMStates.Stopped;
            }
        }
        else
        {
            _logger.LogError("Tag contains ON, but start and end time are the same");
            throw new Exception("Start and end time are the same");
        }

        return VMStates.Omit;
    }

    public async Task<dynamic> GetTagsFromAzure(TimeOnly triggerTime)
    {
        // 1. Get all subscriptions
        // 2. Check if the subscription has the VMStartStop tag
        // - If yes, then switch all VMs on subscription
        // - If no, then:
        // 3. Get all resource groups
        // 4. Check if the resource group has the VMStartStop tag
        // - If yes, then switch all VMs on resource group
        // - If no, then:
        // 5. Get all VMs
        // 6. Check if the VM has the VMStartStop tag and switch them if needed

        var subscriptions = _tagsRepository.GetTagsFromSubscriptions();
        var resourceGroups = _tagsRepository.GetTagsFromResourceGroups();
        var vms = await _tagsRepository.GetTagsFromVirtualMachines();

        var subscriptionsToSwitch = new List<dynamic>();
        var resourceGroupsToSwitch = new List<dynamic>();
        var vmsToSwitch = new List<dynamic>();

        foreach (var subscription in subscriptions)
        {
            var vmState = IsCurrentTag(subscription.TagValue, triggerTime);

            switch (vmState)
            {
                case VMStates.TurningOn:
                    subscriptionsToSwitch.Add(new { SubscriptionId = subscription.Id, Action = "start" });
                    break;

                case VMStates.TurningOff:
                    subscriptionsToSwitch.Add(new { SubscriptionId = subscription.Id, Action = "stop" });
                    break;
            }
        }

        foreach (var resourceGroup in resourceGroups)
        {
            var vmState = IsCurrentTag(resourceGroup.TagValue, triggerTime);

            switch (vmState)
            {
                case VMStates.TurningOn:
                    resourceGroupsToSwitch.Add(new
                    {
                        ResourceGroupName = resourceGroup.Name,
                        SubscriptionId = resourceGroup.SubscriptionId,
                        Action = "start"
                    });
                    break;

                case VMStates.TurningOff:
                    resourceGroupsToSwitch.Add(new
                    {
                        ResourceGroupName = resourceGroup.Name,
                        SubscriptionId = resourceGroup.SubscriptionId,
                        Action = "stop"
                    });
                    break;
            }
        }

        foreach (var vm in vms)
        {
            var vmState = IsCurrentTag(vm.TagValue, triggerTime);

            switch (vmState)
            {
                case VMStates.TurningOn:
                    vmsToSwitch.Add(new
                    {
                        VmId = vm,
                        SubscriptionId = vm.SubscriptionId,
                        ResourceGroupName = vm.ResourceGroupName,
                        Action = "start"
                    });
                    break;

                case VMStates.TurningOff:
                    vmsToSwitch.Add(new
                    {
                        VmId = vm,
                        SubscriptionId = vm.SubscriptionId,
                        ResourceGroupName = vm.ResourceGroupName,
                        Action = "stop"
                    });
                    break;
            }
        }

        return new
        {
            Subscriptions = subscriptionsToSwitch,
            ResourceGroups = resourceGroupsToSwitch,
            VMs = vmsToSwitch
        };
    }
}