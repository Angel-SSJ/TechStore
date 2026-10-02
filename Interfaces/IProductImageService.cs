using TechStore.Models;
using Microsoft.AspNetCore.Http;

namespace TechStore.Interfaces
{
    public interface IProductImageService
    {
        Task AddToProductAsync(Guid productId, ICollection<IFormFile> imageFiles);
        Task RemoveAsync(Guid imageId);
    }
}
