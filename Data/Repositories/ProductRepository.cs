using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechStore.Interfaces;
using TechStore.Models;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Repositories
{
    public class ProductsRepository : Repository<Product, Guid>, IProductsRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Products
                .Include(b => b.Categories)
                .Include(b => b.Images.OrderBy(i => i.ImageNumber))
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public override async Task<IList<Product>> GetAllAsync()
        {
            return await _context.Products
            .Include(product => product.Categories)
                .ToListAsync();
        }

    }
}
