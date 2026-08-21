using Microsoft.AspNetCore.Http;
using TechStore.Models;

namespace TechStore.Interfaces.Services
{
    public interface IProductImageStorageService
    {
        Task<ProductImage> SaveProductImageAsync(Guid productId, IFormFile imageFile);
        Task<IList<ProductImage>> SaveProductImagesAsync(Guid productId, IEnumerable<IFormFile> imageFiles);
        void DeleteProductImage(ProductImage productImage);
        void DeleteAllProductImages(Guid productId);
    }
}
