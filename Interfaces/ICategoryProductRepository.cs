namespace TechStore.Interfaces
{
    public interface ICategoryProductRepository
    {
        Task UpdateAsync(Guid categoryId, IEnumerable<Guid> productIds);
        Task UpdateForProductAsync(Guid productId, IEnumerable<Guid> categoryIds);
    }
}