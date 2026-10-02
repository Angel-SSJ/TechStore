using TechStore.Interfaces;
using TechStore.Models;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Repositories
{
    public class CategoryRepository : Repository<Category, Guid>, ICategoryRespository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdWithProductsAsync(Guid id)
        {
            return await _context.Categories
                .Include(category => category.Products)
                .FirstOrDefaultAsync(category => category.Id == id);
        }

        public override async Task<IList<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(category => category.Products)
                .ToListAsync();
        }

        public Task<bool> HasProductsAsync(Guid id)
        {
            return _context.Categories
                .Where(category => category.Id == id)
                .SelectMany(category => category.Products)
                .AnyAsync();
        }

        public Task<bool> NameExistsAsync(string name, Guid? excludingId = null)
        {
            var normalizedName = name.Trim().ToLower();
            return _context.Categories.AnyAsync(category =>
                category.Name.ToLower() == normalizedName &&
                (!excludingId.HasValue || category.Id != excludingId.Value));
        }
    }
}
