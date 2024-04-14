using Azure.ResourceManager.Resources;

namespace MyCo.TagManager
{
    public interface ITagsRepository
    {
        IEnumerable<Subscription> GetTagsFromSubscriptions();
        IEnumerable<ResourceGroup> GetTagsFromResourceGroups();
        IEnumerable<ResourceGroup> GetTagsFromResourceGroupsInSubscription(SubscriptionResource subscription);
        IEnumerable<VirtualMachine> GetTagsFromVirtualMachinesInResourceGroup();
    }
}