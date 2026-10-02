using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface ICategoryQueries
    {
        Task<IList<Category>> GetAllAsync();
        Task<IList<Category>> GetAllActiveAsync();
        Task<Category?> GetByIdWithProductsAsync(Guid id);
    }
}
