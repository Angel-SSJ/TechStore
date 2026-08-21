using TechStore.Models;

namespace TechStore.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        Task<IList<Product>> GetByCategoryIdAsync(Guid categoryId);
        Task<IList<Product>> SearchByNameAsync(string term);
        Task<IList<Product>> GetLowStockAsync(int threshold);
        Task<IList<Product>> FilterAsync(string? name, Guid? categoryId);
        Task UpdateProductCategoriesAsync(Guid productId, IEnumerable<Guid> categoryIds);
        Task AddImageToProductAsync(Guid productId, ProductImage image);
        Task<ProductImage?> RemoveImageFromProductAsync(Guid productId, Guid imageId);
    }
}
