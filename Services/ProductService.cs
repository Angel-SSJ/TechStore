using TechStore.Interfaces.Repositories;
using TechStore.Interfaces.Services;
using TechStore.Models;

namespace TechStore.Services
{
    public class ProductService : GenericService<Product, Guid>, IProductService
    {
        protected readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) : base(productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public override async Task<Product> AddAsync(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(entity.Name));
            }

            if (entity.Price < 0)
            {
                throw new ArgumentException("El precio del producto no puede ser negativo.", nameof(entity.Price));
            }

            if (entity.Stock < 0)
            {
                throw new ArgumentException("El stock del producto no puede ser negativo.", nameof(entity.Stock));
            }

            return await base.AddAsync(entity);
        }

        public override async Task<Product> UpdateAsync(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(entity.Name));
            }

            if (entity.Price < 0)
            {
                throw new ArgumentException("El precio del producto no puede ser negativo.", nameof(entity.Price));
            }

            if (entity.Stock < 0)
            {
                throw new ArgumentException("El stock del producto no puede ser negativo.", nameof(entity.Stock));
            }

            return await base.UpdateAsync(entity);
        }

        public async Task<IList<Product>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _productRepository.GetByCategoryIdAsync(categoryId);
        }

        public async Task<IList<Product>> SearchByNameAsync(string term)
        {
            return await _productRepository.SearchByNameAsync(term);
        }

        public async Task<IList<Product>> GetLowStockAsync(int threshold)
        {
            if (threshold < 0)
            {
                threshold = 0;
            }

            return await _productRepository.GetLowStockAsync(threshold);
        }

        public async Task<IList<Product>> FilterAsync(string? name, Guid? categoryId)
        {
            return await _productRepository.FilterAsync(name, categoryId);
        }

        public async Task UpdateProductCategoriesAsync(Guid productId, IEnumerable<Guid> categoryIds)
        {
            await _productRepository.UpdateProductCategoriesAsync(productId, categoryIds);
        }

        public async Task AddImageToProductAsync(Guid productId, ProductImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            await _productRepository.AddImageToProductAsync(productId, image);
        }

        public async Task<ProductImage?> RemoveImageFromProductAsync(Guid productId, Guid imageId)
        {
            return await _productRepository.RemoveImageFromProductAsync(productId, imageId);
        }
    }
}
