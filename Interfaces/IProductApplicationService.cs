using Microsoft.AspNetCore.Http;
using TechStore.DTOs;
using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface IProductApplicationService
    {
        Task<Product> CreateAsync(ProductInput input, IEnumerable<Guid>? categoryIds, ICollection<IFormFile>? images);
        Task<Product?> UpdateAsync(Guid id, ProductInput input, IEnumerable<Guid> categoryIds, ICollection<IFormFile>? images);
        Task AddImagesAsync(Guid productId, ICollection<IFormFile> images);
        Task RemoveImageAsync(Guid imageId);
    }
}