using TechStore.DTOs;
using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface ICategoryApplicationService
    {
        Task<Category> CreateAsync(CategoryInput input, IEnumerable<Guid>? productIds);
        Task<Category?> UpdateAsync(Guid id, CategoryInput input, IEnumerable<Guid> productIds);
    }
}
