using TechStore.Interfaces;

namespace TechStore.Interfaces.Services
{
    public interface IService<T, I> where T : IEntity<I>
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(I id);
        Task<T> RestoreAsync(I id);
        Task<T> AlreadyExistsByIdAsync(I id);
        Task<T?> GetByIdAsync(I id);
        Task<IList<T>> GetAllAsync();
        Task<IList<T>> GetAllActiveAsync();
        Task<IList<T>> GetAllInactiveAsync();
    }
}
