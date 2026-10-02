using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface IProductImageRepository
    {
        Task<ProductImage> AddAsync(ProductImage image);
        Task<ProductImage?> GetByIdAsync(Guid imageId);
        Task RemoveAsync(ProductImage image);
    }
}
