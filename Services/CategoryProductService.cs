using TechStore.Interfaces;

namespace TechStore.Services
{
    public class CategoryProductService : ICategoryProductService
    {
        private readonly ICategoryProductRepository _repository;

        public CategoryProductService(ICategoryProductRepository repository)
        {
            _repository = repository;
        }

        public Task UpdateAsync(Guid categoryId, IEnumerable<Guid> productIds)
        {
            return _repository.UpdateAsync(categoryId, productIds);
        }

        public Task UpdateForProductAsync(Guid productId, IEnumerable<Guid> categoryIds)
        {
            return _repository.UpdateForProductAsync(productId, categoryIds);
        }
    }
}
