using TechStore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Repositories
{
    public class CategoryProductRepository : ICategoryProductRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Guid categoryId, IEnumerable<Guid> productsIds)
        {
            var category = await _context.Categories
                .Include(item => item.Products)
                .FirstOrDefaultAsync(item => item.Id == categoryId);

            if (category == null)
            {
                return;
            }

            var selectedProducts = await _context.Products
                .Where(product => productsIds.Distinct().Contains(product.Id))
                .ToListAsync();

            var selectedProductsIdSet = selectedProducts.Select(product => product.Id).ToHashSet();
            var removedProductIds = category.Products
                .Select(product => product.Id)
                .Where(productId => !selectedProductsIdSet.Contains(productId))
                .ToList();

            if (removedProductIds.Count > 0)
            {
                var productsWithOtherCategories = await _context.Products
                    .Where(product => removedProductIds.Contains(product.Id) && product.Categories.Count() > 1)
                    .Select(product => product.Id)
                    .ToListAsync();

                var invalidProductIds = removedProductIds.Except(productsWithOtherCategories).ToList();
                if (invalidProductIds.Count > 0)
                {
                    throw new InvalidOperationException("No puedes quitar la última categoría de un producto.");
                }
            }

            category.ReplaceProducts(selectedProducts);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateForProductAsync(Guid productId, IEnumerable<Guid> categoryIds)
        {
            var product = await _context.Products
                .Include(item => item.Categories)
                .FirstOrDefaultAsync(item => item.Id == productId);

            if (product == null)
            {
                return;
            }

            var selectedCategories = await _context.Categories
                .Where(category => categoryIds.Distinct().Contains(category.Id))
                .ToListAsync();

            product.ReplaceCategories(selectedCategories);
            await _context.SaveChangesAsync();
        }
    }
}
