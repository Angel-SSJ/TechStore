using TechStore.Models;

namespace TechStore.Interfaces.Services
{
    public interface ICategoryService : IService<Category, Guid>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
