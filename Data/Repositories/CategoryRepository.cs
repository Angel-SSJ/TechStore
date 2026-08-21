using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Interfaces;
using TechStore.Interfaces.Repositories;
using TechStore.Models;

namespace TechStore.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.CategoryProducts)
                    .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IList<Category>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.CategoryProducts)
                    .ThenInclude(cp => cp.Product)
                .ToListAsync();
        }

        public override async Task<IList<Category>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .Include(c => c.CategoryProducts)
                    .ThenInclude(cp => cp.Product)
                .ToListAsync();
        }

        public override async Task<IList<Category>> GetAllInactiveAsync()
        {
            return await _dbSet
                .Where(c => !c.IsActive)
                .Include(c => c.CategoryProducts)
                    .ThenInclude(cp => cp.Product)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(c => c.CategoryProducts)
                    .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }
}
