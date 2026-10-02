using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface IProductQueries
    {
        Task<IList<Product>> GetAllAsync();
        Task<Product?> GetByIdWithDetailsAsync(Guid id);
    }
}
