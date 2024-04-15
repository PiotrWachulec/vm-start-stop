using Azure.ResourceManager.Resources;

namespace MyCo.TagManager
{
    public interface ITagsRepository
    {
        IEnumerable<Subscription> GetTagsFromSubscriptions();
        IEnumerable<ResourceGroup> GetTagsFromResourceGroups();
        IEnumerable<ResourceGroup> GetTagsFromResourceGroupsInSubscription(SubscriptionResource subscription);
        Task<IEnumerable<VirtualMachine>> GetTagsFromVirtualMachines();
        Task<IEnumerable<VirtualMachine>> GetTagsFromVirtualMachinesInSubscription(SubscriptionResource subscription);
    }
}