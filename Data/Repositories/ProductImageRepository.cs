using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Interfaces;
using TechStore.Models;

namespace TechStore.Data.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

         public async Task<ProductImage> AddAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<ProductImage?> GetByIdAsync(Guid imageId)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(image => image.Id == imageId);
        }

        public async Task RemoveAsync(ProductImage image)
        {
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }
}