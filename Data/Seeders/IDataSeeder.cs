using TechStore.Data;

namespace TechStore.Data.Seeders
{
    public interface IDataSeeder
    {
        int Order { get; }
        Task SeedAsync(ApplicationDbContext context);
    }
}
