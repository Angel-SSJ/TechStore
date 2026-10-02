namespace TechStore.Interfaces
{
    public interface IEntityLifecycleService<T, I> where T : IEntity<I>
    {
        Task<bool> DeleteAsync(I id);
        Task<bool> RestoreAsync(I id);
        Task<bool> AlreadyExistsAsync(I id);
    }
}
