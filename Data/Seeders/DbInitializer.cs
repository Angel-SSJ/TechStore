using Microsoft.EntityFrameworkCore;
using TechStore.Data;

namespace TechStore.Data.Seeders
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, bool applyMigrations = true)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetService<ILogger<ApplicationDbContext>>();

            try
            {
                if (applyMigrations)
                {
                    logger?.LogInformation("Verificando y aplicando migraciones pendientes en la base de datos...");
                    await context.Database.MigrateAsync();
                }

                logger?.LogInformation("Ejecutando Seeders registrados...");
                var seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
                    .OrderBy(s => s.Order)
                    .ToList();

                foreach (var seeder in seeders)
                {
                    logger?.LogInformation("Ejecutando seeder: {SeederName}", seeder.GetType().Name);
                    await seeder.SeedAsync(context);
                }

                logger?.LogInformation("Inicialización y sembrado de base de datos completado.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error durante la inicialización o sembrado de la base de datos.");
                throw;
            }
        }
    }
}
