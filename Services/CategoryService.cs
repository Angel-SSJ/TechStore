using TechStore.Interfaces.Repositories;
using TechStore.Interfaces.Services;
using TechStore.Models;

namespace TechStore.Services
{
    public class CategoryService : GenericService<Category, Guid>, ICategoryService
    {
        protected readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository) : base(categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public override async Task<Category> AddAsync(Category entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(entity.Name));
            }

            if (await _categoryRepository.ExistsByNameAsync(entity.Name))
            {
                throw new InvalidOperationException($"Ya existe una categoría con el nombre '{entity.Name}'.");
            }

            return await base.AddAsync(entity);
        }

        public override async Task<Category> UpdateAsync(Category entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(entity.Name));
            }

            return await base.UpdateAsync(entity);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return await _categoryRepository.GetByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return await _categoryRepository.ExistsByNameAsync(name);
        }
    }
}
