namespace TechStore.Interfaces
{
    public interface IProductLifecycle
    {
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
    }
}
