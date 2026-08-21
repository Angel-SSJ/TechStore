using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Data.Repositories;
using TechStore.Data.Seeders;
using TechStore.Interfaces.Repositories;
using TechStore.Interfaces.Services;
using TechStore.Services;
using System.Text.Json.Serialization;
using TechStore.Evaluation;

if (args.Contains("--test") || args.Contains("--eval"))
{
    var success = await ControllerEvaluator.RunAllEvaluationsAsync();
    return;
}

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos (SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Controladores y Configuración JSON
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 3. Inyección de Dependencias de Repositorios
builder.Services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// 4. Inyección de Dependencias de Servicios
builder.Services.AddScoped(typeof(IService<,>), typeof(GenericService<,>));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IProductImageStorageService, ProductImageStorageService>();

// 5. Inyección de Dependencias de Seeders (Modular y Escalable)
builder.Services.AddScoped<IDataSeeder, CategorySeeder>();
builder.Services.AddScoped<IDataSeeder, ProductSeeder>();

var app = builder.Build();

// 6. Aplicar Migraciones y Sembrado de Datos Automático en Desarrollo o con flag --seed
if (app.Environment.IsDevelopment() || args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.InitializeAsync(services, applyMigrations: true);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar migraciones o sembrar la base de datos.");
    }
}

// Configuración del Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.MapStaticAssets();

// Mapeo de Controladores
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
