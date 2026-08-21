using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Data.Seeders
{
    public class CategorySeeder : IDataSeeder
    {
        public int Order => 1;

        public async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            var categoryNames = new[]
            {
                "Laptops & MacBooks",
                "Smartphones & iPhones",
                "Tablets & iPads",
                "Fundas & Protectores (Cases)",
                "Cargadores & Cables",
                "Smartwatches & Wearables",
                "Audio & Auriculares",
                "Monitores & Pantallas",
                "Almacenamiento & Memorias",
                "Periféricos & Gaming"
            };

            var categories = categoryNames.Select(name => new Category
            {
                Name = name.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }).ToList();

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
