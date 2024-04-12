namespace MyCo.TagManager
{
    public interface ITagsRepository
    {
        Task<IEnumerable<Subscription>> GetTagsFromSubscriptions();
    }
}