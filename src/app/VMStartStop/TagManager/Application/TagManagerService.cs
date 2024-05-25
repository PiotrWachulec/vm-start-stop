using Microsoft.Extensions.Logging;
using MyCo.TagManager.Domain;

namespace MyCo.TagManager.Application;

public class TagManagerService : ITagManagerService
{
    private readonly ILogger _logger;
    private readonly ITagsRepository _tagsRepository;

    public TagManagerService(ILoggerFactory loggerFactory, ITagsRepository tagsRepository)
    {
        _logger = loggerFactory.CreateLogger<TagManagerService>();
        _tagsRepository = tagsRepository;
    }

    public VMStates IsCurrentTag(VMStartStopTagValue tagValue, TimeOnly currentTime)
    {
        if (!tagValue.IsOn)
        {
            _logger.LogInformation("Tag contains OFF, omitting VM");
            return VMStates.Omit;
        }

        if (tagValue.StartTime.CompareTo(tagValue.EndTime) < 0)
        {
            switch (currentTime)
            {
                case var _ when currentTime < tagValue.StartTime || currentTime > tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, but it's not time to start VM");
                    return VMStates.Stopped;

                case var _ when currentTime == tagValue.StartTime:
                    _logger.LogInformation("Tag contains ON, VM is starting");
                    return VMStates.TurningOn;

                case var _ when currentTime > tagValue.StartTime && currentTime < tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, VM is running");
                    return VMStates.Running;

                case var _ when currentTime == tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, VM is stopping");
                    return VMStates.TurningOff;
            }
        }
        else if (tagValue.StartTime.CompareTo(tagValue.EndTime) > 0)
        {
            switch (currentTime)
            {
                case var _ when currentTime == tagValue.StartTime:
                    _logger.LogInformation("Tag contains ON, VM is starting");
                    return VMStates.TurningOn;

                case var _ when currentTime == tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, VM is stopping");
                    return VMStates.TurningOff;

                case var _ when currentTime > tagValue.StartTime || currentTime < tagValue.EndTime:
                    _logger.LogInformation("Tag contains ON, but it's not time to stop VM");
                    return VMStates.Running;

                case var _ when currentTime > tagValue.EndTime && currentTime < tagValue.StartTime:
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

    public async Task GetTagsFromAzure()
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
    }
}