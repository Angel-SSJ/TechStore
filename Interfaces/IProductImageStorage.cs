using TechStore.Models;
using Microsoft.AspNetCore.Http;

namespace TechStore.Interfaces
{
    public sealed record StoredProductImage(string ImagePath, int ImageNumber, long FileSize);

    public interface IProductImageStorage
    {
        Task<StoredProductImage> SaveAsync(Guid productId, IFormFile imageFile);
        void Delete(ProductImage productImage);
    }
}
