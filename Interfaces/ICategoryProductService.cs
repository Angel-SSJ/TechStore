namespace TechStore.Interfaces
{
    public interface ICategoryProductService
    {
        Task UpdateAsync(Guid categoryId, IEnumerable<Guid> productIds);
        Task UpdateForProductAsync(Guid productId, IEnumerable<Guid> categoryIds);
    }
}