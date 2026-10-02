using Microsoft.AspNetCore.Http;
using TechStore.DTOs;
using TechStore.Interfaces;
using TechStore.Models;

namespace TechStore.Services
{
    public sealed class ProductApplicationService : IProductApplicationService
    {
        private readonly IProductsService _products;
        private readonly ICategoryProductService _categoryProducts;
        private readonly IProductImageService _images;

        public ProductApplicationService(IProductsService products, ICategoryProductService categoryProducts, IProductImageService images)
        {
            _products = products;
            _categoryProducts = categoryProducts;
            _images = images;
        }

        public async Task<Product> CreateAsync(ProductInput input, IEnumerable<Guid>? categoryIds, ICollection<IFormFile>? images)
        {
            var product = new Product(input.Name.Trim(), input.Description.Trim(), input.Price, input.Stock, input.Featured, input.IsActive);
            product.MarkCreated();
            var created = await _products.AddAsync(product);
            await _categoryProducts.UpdateForProductAsync(created.Id, categoryIds ?? Enumerable.Empty<Guid>());
            if (images is { Count: > 0 }) await _images.AddToProductAsync(created.Id, images);
            return created;
        }

        public async Task<Product?> UpdateAsync(Guid id, ProductInput input, IEnumerable<Guid> categoryIds, ICollection<IFormFile>? images)
        {
            var product = await _products.GetByIdAsync(id);
            if (product == null) return null;

            product.UpdateDetails(input.Name.Trim(), input.Description.Trim(), input.Price, input.Stock, input.Featured);
            if (input.IsActive) product.Activate(); else product.Deactivate();
            var updated = await _products.UpdateAsync(product);
            await _categoryProducts.UpdateForProductAsync(id, categoryIds);
            if (images is { Count: > 0 }) await _images.AddToProductAsync(id, images);
            return updated;
        }

        public Task AddImagesAsync(Guid productId, ICollection<IFormFile> images) => _images.AddToProductAsync(productId, images);

        public Task RemoveImageAsync(Guid imageId) => _images.RemoveAsync(imageId);
    }
}