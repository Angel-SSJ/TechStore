using TechStore.Models;

namespace TechStore.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
