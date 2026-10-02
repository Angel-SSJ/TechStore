using TechStore.DTOs;
using TechStore.Interfaces;
using TechStore.Models;

namespace TechStore.Services
{
    public class CategoryService : Service<Category, Guid>, ICategoryService
    {
        private readonly ICategoryRespository _repository;
        private readonly ICategoryProductService _categoryProductService;

        public CategoryService(
            ICategoryRespository repository,
            ICategoryProductService categoryProductService) : base(repository, repository, repository)
        {
            _repository = repository;
            _categoryProductService = categoryProductService;
        }

        public async Task<Category> CreateAsync(CategoryInput input, IEnumerable<Guid>? productIds)
        {
            await EnsureUniqueNameAsync(input.Name);

            var category = new Category();
            category.UpdateDetails(input.Name.Trim(), input.Description.Trim());
            if (!input.IsActive)
            {
                category.Deactivate();
            }

            category.MarkCreated();
            var createdCategory = await AddAsync(category);
            await _categoryProductService.UpdateAsync(createdCategory.Id, productIds ?? Enumerable.Empty<Guid>());
            return createdCategory;
        }

        public async Task<Category?> UpdateAsync(Guid id, CategoryInput input, IEnumerable<Guid> productIds)
        {
            var existingCategory = await GetByIdAsync(id);
            if (existingCategory == null)
            {
                return null;
            }

            await EnsureUniqueNameAsync(input.Name, id);
            existingCategory.UpdateDetails(input.Name.Trim(), input.Description.Trim());
            if (input.IsActive)
            {
                existingCategory.Activate();
            }
            else
            {
                existingCategory.Deactivate();
            }

            var updatedCategory = await base.UpdateAsync(existingCategory);
            await _categoryProductService.UpdateAsync(id, productIds);
            return updatedCategory;
        }

        public async Task<Category?> GetByIdWithProductsAsync(Guid id)
        {
            return await _repository.GetByIdWithProductsAsync(id);
        }

        public new Task<IList<Category>> GetAllActiveAsync()
        {
            return base.GetAllActiveAsync();
        }

        public new async Task<bool> DeleteAsync(Guid id)
        {
            if (await _repository.HasProductsAsync(id))
            {
                return false;
            }

            return await base.DeleteAsync(id);
        }

        private async Task EnsureUniqueNameAsync(string name, Guid? excludingId = null)
        {
            if (await _repository.NameExistsAsync(name, excludingId))
            {
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
            }
        }
    }
}
