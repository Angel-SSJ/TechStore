using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Interfaces;
using TechStore.Interfaces.Repositories;
using TechStore.Models;

namespace TechStore.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IList<Product>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .ToListAsync();
        }

        public override async Task<IList<Product>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .ToListAsync();
        }

        public override async Task<IList<Product>> GetAllInactiveAsync()
        {
            return await _dbSet
                .Where(p => !p.IsActive)
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .ToListAsync();
        }

        public async Task<IList<Product>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _dbSet
                .Where(p => p.IsActive && p.Categories.Any(cp => cp.CategoryId == categoryId))
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .ToListAsync();
        }

        public async Task<IList<Product>> SearchByNameAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return await GetAllActiveAsync();
            }

            return await _dbSet
                .Where(p => p.IsActive && p.Name.ToLower().Contains(term.ToLower()))
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .ToListAsync();
        }

        public async Task<IList<Product>> GetLowStockAsync(int threshold)
        {
            return await _dbSet
                .Where(p => p.IsActive && p.Stock <= threshold)
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .ToListAsync();
        }

        public async Task<IList<Product>> FilterAsync(string? name, Guid? categoryId)
        {
            var query = _dbSet.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }

            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            {
                query = query.Where(p => p.Categories.Any(cp => cp.CategoryId == categoryId.Value));
            }

            return await query
                .Include(p => p.Images)
                .Include(p => p.Categories)
                    .ThenInclude(cp => cp.Category)
                .ToListAsync();
        }

        public async Task UpdateProductCategoriesAsync(Guid productId, IEnumerable<Guid> categoryIds)
        {
            var product = await _dbSet.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontró ningún producto con el ID '{productId}'.");
            }

            _context.CategoryProducts.RemoveRange(product.Categories);

            if (categoryIds != null)
            {
                foreach (var catId in categoryIds.Distinct())
                {
                    product.Categories.Add(new CategoryProduct
                    {
                        ProductId = productId,
                        CategoryId = catId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddImageToProductAsync(Guid productId, ProductImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var product = await _dbSet.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontró ningún producto con el ID '{productId}'.");
            }

            product.Images.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductImage?> RemoveImageFromProductAsync(Guid productId, Guid imageId)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(pi => pi.Id == imageId && pi.ProductId == productId);
            if (image == null)
            {
                return null;
            }

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return image;
        }
    }
}
