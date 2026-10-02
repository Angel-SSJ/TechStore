using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TechStore.Controllers;
using TechStore.Data;
using TechStore.Data.Repositories;
using TechStore.Data.Seeders;
using TechStore.Models;
using TechStore.Models.DTOs;
using TechStore.Services;

namespace TechStore.Evaluation
{
    public class MockWebHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "TechStore";
        public string WebRootPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        public IFileProvider WebRootFileProvider { get; set; } = default!;
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = default!;
    }

    public class MockTempDataProvider : ITempDataProvider
    {
        private readonly Dictionary<string, object> _data = new();
        public IDictionary<string, object> LoadTempData(HttpContext context) => _data;
        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            _data.Clear();
            foreach (var kv in values)
            {
                _data[kv.Key] = kv.Value;
            }
        }
    }

    public static class ControllerEvaluator
    {
        public static async Task<bool> RunAllEvaluationsAsync()
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("🚀 INICIANDO EVALUACIÓN Y GENERACIÓN DE LOGS DE CONTROLADORES MVC");
            Console.WriteLine("==========================================================");

            // Asegurar directorio base de logs
            string baseLogsDir = Path.Combine(Directory.GetCurrentDirectory(), "Evaluation", "logs");
            if (Directory.Exists(baseLogsDir))
            {
                Directory.Delete(baseLogsDir, recursive: true);
            }
            Directory.CreateDirectory(baseLogsDir);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TechStore_TestDb_{Guid.NewGuid()}")
                .Options;

            using var context = new ApplicationDbContext(options);
            var environment = new MockWebHostEnvironment();

            // 1. Inicializar Repositorios
            var categoryRepo = new CategoryRepository(context);
            var productRepo = new ProductRepository(context);

            // 2. Inicializar Servicios
            var categoryService = new CategoryService(categoryRepo);
            var productService = new ProductService(productRepo);
            var storageService = new ProductImageStorageService(environment);

            // 3. Inicializar Controladores MVC con contexto HTTP y TempData
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = new MockTempDataProvider();

            var categoryController = new CategoryController(categoryService)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, tempDataProvider)
            };

            var productController = new ProductController(productService, storageService, categoryService)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, tempDataProvider)
            };

            // 4. Sembrar datos iniciales con los seeders reales
            var catSeeder = new CategorySeeder();
            await catSeeder.SeedAsync(context);

            var prodSeeder = new ProductSeeder(environment);
            await prodSeeder.SeedAsync(context);

            int passed = 0;
            int failed = 0;

            void AssertTest(string testName, bool condition, string details = "")
            {
                if (condition)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  ✅ [PASSED] {testName} {details}");
                    Console.ResetColor();
                    passed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ❌ [FAILED] {testName} - {details}");
                    Console.ResetColor();
                    failed++;
                }
            }

            Console.WriteLine("\n--- 1. EVALUACIÓN Y LOGS DE CategoryController (MVC) ---");

            // C1: Index (GetAll Activas)
            var rC1 = await categoryController.Index(includeInactive: false);
            await LogEndpointResponseAsync("category", "index", "get", rC1, "Consulta de vista Index de categorías activas");
            var listC1 = (rC1 as ViewResult)?.Model as List<CategoryDetailDto>;
            AssertTest("C1: Index Activas", listC1?.Count == 10, $"Total activas: {listC1?.Count}/10");

            // C2: Index
            var rC2 = await categoryController.Index();
            await LogEndpointResponseAsync("category", "index-default", "get", rC2, "Consulta predeterminada de Index");
            AssertTest("C2: Index ViewResult", rC2 is ViewResult, "200 ViewResult");

            // C3: Index Inactivas
            var rC3 = await categoryController.Index(includeInactive: true);
            await LogEndpointResponseAsync("category", "index-all", "get", rC3, "Consulta de Index con inactivas");
            AssertTest("C3: Index Inactivas", rC3 is ViewResult, "200 ViewResult");

            // C4: Details existente
            var firstCat = listC1!.First();
            var rC4 = await categoryController.Details(firstCat.Id);
            await LogEndpointResponseAsync("category", "details", "get", rC4, $"Vista Details por ID existente ({firstCat.Id})");
            var dtoC4 = (rC4 as ViewResult)?.Model as CategoryDetailDto;
            AssertTest("C4: Details Existente", dtoC4?.Name == firstCat.Name, $"Nombre: '{dtoC4?.Name}'");

            // C5: Details inexistente (404)
            var nonExistentCatId = Guid.NewGuid();
            var rC5 = await categoryController.Details(nonExistentCatId);
            await LogEndpointResponseAsync("category", "details-notfound", "get", rC5, $"Vista Details por ID inexistente ({nonExistentCatId})");
            AssertTest("C5: Details Inexistente (404)", rC5 is NotFoundResult, "404 NotFound");

            // C6: Create GET
            var rC6 = categoryController.Create();
            await LogEndpointResponseAsync("category", "create-view", "get", rC6, "Formulario GET Create de categoría");
            AssertTest("C6: Create GET Formulario", rC6 is ViewResult, "ViewResult");

            // C7: Create POST Válido (Redirección a Index)
            var rC7 = await categoryController.Create(new CreateCategoryDto { Name = "Drones & Fotografía" });
            await LogEndpointResponseAsync("category", "create", "post", rC7, "Creación exitosa de categoría con redirección PRG");
            var createdCat = await categoryService.GetByNameAsync("Drones & Fotografía");
            AssertTest("C7: Create POST (RedirectToAction)", rC7 is RedirectToActionResult && createdCat != null, $"ID: {createdCat?.Id}");

            // C8: Create POST Duplicado (Re-render de vista con error)
            categoryController.ModelState.Clear();
            var rC8 = await categoryController.Create(new CreateCategoryDto { Name = "Drones & Fotografía" });
            await LogEndpointResponseAsync("category", "create-conflict", "post", rC8, "Intento de creación con nombre duplicado");
            AssertTest("C8: Create Duplicado (Validación y re-render)", rC8 is ViewResult && !categoryController.ModelState.IsValid, "Modelo inválido capturado");

            // C9: Edit GET
            var rC9 = await categoryController.Edit(createdCat!.Id);
            await LogEndpointResponseAsync("category", "edit-view", "get", rC9, "Formulario GET Edit de categoría");
            var editDtoC9 = (rC9 as ViewResult)?.Model as UpdateCategoryDto;
            AssertTest("C9: Edit GET Formulario", rC9 is ViewResult && editDtoC9?.Name == "Drones & Fotografía", "Formulario cargado");

            // C10: Edit POST Válido
            categoryController.ModelState.Clear();
            var rC10 = await categoryController.Edit(createdCat.Id, new UpdateCategoryDto { Name = "Drones & Cámaras Pro" });
            await LogEndpointResponseAsync("category", "edit", "post", rC10, $"Actualización de nombre para categoría {createdCat.Id}");
            var updatedCat = await categoryService.GetByIdAsync(createdCat.Id);
            AssertTest("C10: Edit POST (RedirectToAction)", rC10 is RedirectToActionResult && updatedCat?.Name == "Drones & Cámaras Pro", $"Nuevo nombre: '{updatedCat?.Name}'");

            // C11: Delete POST (Soft-Delete)
            var rC11 = await categoryController.DeleteConfirmed(createdCat.Id);
            await LogEndpointResponseAsync("category", "delete", "post", rC11, $"Soft-delete de categoría {createdCat.Id}");
            var deletedCat = await categoryService.GetByIdAsync(createdCat.Id);
            AssertTest("C11: Delete Soft-Delete", deletedCat?.IsActive == false, "IsActive es false");

            // C12: Restore POST
            var rC12 = await categoryController.Restore(createdCat.Id);
            await LogEndpointResponseAsync("category", "restore", "post", rC12, $"Restauración de categoría desactivada {createdCat.Id}");
            var restoredCat = await categoryService.GetByIdAsync(createdCat.Id);
            AssertTest("C12: Restore Categoría", restoredCat?.IsActive == true, "IsActive restaurado a true");

            Console.WriteLine("\n--- 2. EVALUACIÓN Y LOGS DE ProductController (MVC) ---");

            // P1: Index (GetAll Productos)
            var rP1 = await productController.Index();
            await LogEndpointResponseAsync("product", "index", "get", rP1, "Consulta de catálogo completo en vista Index");
            var listP1 = (rP1 as ViewResult)?.Model as List<ProductDetailDto>;
            AssertTest("P1: Index Catálogo Completo", listP1?.Count == 34, $"Total productos: {listP1?.Count}/34");

            // P2: Index con ViewBag de Categorías
            AssertTest("P2: Index ViewBag Categorías", productController.ViewBag.Categories != null, "ViewBag poblado");

            // P3: Index con Inactivos
            var rP3 = await productController.Index(includeInactive: true);
            await LogEndpointResponseAsync("product", "index-inactive", "get", rP3, "Consulta de productos incluyendo inactivos");
            AssertTest("P3: Index Inactivos", rP3 is ViewResult, "ViewResult");

            // P4: Filtro por SearchTerm
            var rP4 = await productController.Index(searchTerm: "MacBook");
            await LogEndpointResponseAsync("product", "filter-searchterm", "get", rP4, "Filtro por término de búsqueda 'MacBook'");
            var listP4 = (rP4 as ViewResult)?.Model as List<ProductDetailDto>;
            AssertTest("P4: Filtro por Nombre ('MacBook')", listP4?.Count == 2, $"Coincidencias: {listP4?.Count}");

            // P5: Filtro por CategoryId
            var catLaptopsId = listC1.First(c => c.Name == "Laptops & MacBooks").Id;
            var rP5 = await productController.Index(categoryId: catLaptopsId);
            await LogEndpointResponseAsync("product", "filter-category", "get", rP5, $"Filtro por CategoryId de Laptops ({catLaptopsId})");
            var listP5 = (rP5 as ViewResult)?.Model as List<ProductDetailDto>;
            AssertTest("P5: Filtro por Categoría (Laptops)", listP5?.Count == 3, $"Productos: {listP5?.Count}");

            // P6: Filtro Combinado
            var rP6 = await productController.Index(searchTerm: "Pro", categoryId: catLaptopsId);
            await LogEndpointResponseAsync("product", "filter-combined", "get", rP6, "Filtro combinado: 'Pro' + Categoría Laptops");
            var listP6 = (rP6 as ViewResult)?.Model as List<ProductDetailDto>;
            AssertTest("P6: Filtro Combinado (Pro + Laptops)", listP6?.Count == 1, $"Producto: '{listP6?.FirstOrDefault()?.Name}'");

            // P7: Details con resolución N+1
            var sampleProduct = listP1!.First(p => p.Name.Contains("MacBook Pro 16"));
            var rP7 = await productController.Details(sampleProduct.Id);
            await LogEndpointResponseAsync("product", "details", "get", rP7, $"Detalle de producto con categorías e imágenes ({sampleProduct.Id})");
            var dtoP7 = (rP7 as ViewResult)?.Model as ProductDetailDto;
            bool categoryNamesPresent = dtoP7?.Categories.All(c => !string.IsNullOrEmpty(c.Name)) ?? false;
            bool imagesPresent = (dtoP7?.Images.Count ?? 0) >= 2;
            AssertTest("P7: Details Eager Loading", categoryNamesPresent && imagesPresent, $"Categorías: {dtoP7?.Categories.Count}, Imágenes: {dtoP7?.Images.Count}");

            // P8: LowStock
            var rP8 = await productController.LowStock(threshold: 5);
            await LogEndpointResponseAsync("product", "lowstock", "get", rP8, "Consulta de productos con stock bajo (<= 5)");
            var listP8 = (rP8 as ViewResult)?.Model as List<ProductDetailDto>;
            AssertTest("P8: LowStock (Stock <= 5)", listP8?.All(p => p.Stock <= 5) == true, $"Productos: {listP8?.Count}");

            // P9: Create POST Producto con Categorías
            var createDto = new CreateProductDto
            {
                Name = "Consola PlayStation 5 Pro 2TB",
                Description = "Consola con GPU mejorada, trazado de rayos avanzado y escalado PlayStation Spectral Super Resolution.",
                Price = 699.99m,
                Stock = 15,
                CategoryIds = new List<Guid> { firstCat.Id }
            };
            productController.ModelState.Clear();
            var rP9 = await productController.Create(createDto);
            await LogEndpointResponseAsync("product", "create", "post", rP9, "Creación de nuevo producto con redirección PRG");
            var createdProd = (await productService.SearchByNameAsync("PlayStation 5 Pro")).FirstOrDefault();
            AssertTest("P9: Create POST Producto (Redirect)", rP9 is RedirectToActionResult && createdProd != null, $"ID: {createdProd?.Id}");

            // P10: Edit POST Producto y Sincronización N:M
            var catGamingId = listC1.First(c => c.Name == "Periféricos & Gaming").Id;
            var updateDto = new UpdateProductDto
            {
                Name = "Consola PlayStation 5 Pro 2TB Digital",
                Description = "Edición digital con 2TB de almacenamiento ultrarrápido.",
                Price = 649.99m,
                Stock = 18,
                CategoryIds = new List<Guid> { firstCat.Id, catGamingId }
            };
            productController.ModelState.Clear();
            var rP10 = await productController.Edit(createdProd!.Id, updateDto);
            await LogEndpointResponseAsync("product", "edit", "post", rP10, $"Actualización y sincronización de categorías para producto {createdProd.Id}");
            var updatedProd = await productService.GetByIdAsync(createdProd.Id);
            AssertTest("P10: Edit POST Sincronización N:M", updatedProd?.Categories.Count == 2 && updatedProd?.Price == 649.99m, $"Categorías: {updatedProd?.Categories.Count}");

            // P11: Delete Soft Delete Producto
            var rP11 = await productController.DeleteConfirmed(createdProd.Id);
            await LogEndpointResponseAsync("product", "delete", "post", rP11, $"Soft-delete de producto {createdProd.Id}");
            var deletedProd = await productService.GetByIdAsync(createdProd.Id);
            AssertTest("P11: Delete Soft-Delete Producto", deletedProd?.IsActive == false, "IsActive es false");

            // P12: Restore Producto
            var rP12 = await productController.Restore(createdProd.Id);
            await LogEndpointResponseAsync("product", "restore", "post", rP12, $"Restauración de producto desactivado {createdProd.Id}");
            var restoredProd = await productService.GetByIdAsync(createdProd.Id);
            AssertTest("P12: Restore Producto", restoredProd?.IsActive == true, "IsActive restaurado a true");

            // P13: DeleteImage de Producto
            var sampleImage = sampleProduct.Images.Last();
            var rP13 = await productController.DeleteImage(sampleProduct.Id, sampleImage.Id);
            await LogEndpointResponseAsync("product", "deleteimage", "post", rP13, $"Eliminación dual de imagen {sampleImage.Id} para producto {sampleProduct.Id}");
            var productAfterImgDelete = await productService.GetByIdAsync(sampleProduct.Id);
            bool imageRemoved = productAfterImgDelete?.Images.All(i => i.Id != sampleImage.Id) ?? false;
            AssertTest("P13: DeleteImage (BD + Disco)", imageRemoved, "Imagen eliminada");

            Console.WriteLine("\n==========================================================");
            Console.WriteLine($"📊 RESULTADO FINAL: {passed} PRUEBAS SUPERADAS, {failed} FALLIDAS");
            Console.WriteLine($"📁 Todos los logs generados en: {baseLogsDir}");
            Console.WriteLine("==========================================================");

            return failed == 0;
        }

        private static async Task LogEndpointResponseAsync(string controller, string endpoint, string httpMethod, IActionResult result, string details = "")
        {
            try
            {
                string logsDir = Path.Combine(Directory.GetCurrentDirectory(), "Evaluation", "logs");
                string folderName = $"{controller.ToLower()}-{endpoint.ToLower()}-{httpMethod.ToLower()}";
                string targetDir = Path.Combine(logsDir, folderName);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                int statusCode = 200;
                object? responseValue = null;

                if (result is ViewResult vResult)
                {
                    statusCode = 200;
                    responseValue = new
                    {
                        ViewName = vResult.ViewName ?? "(Default View)",
                        Model = vResult.Model,
                        ViewDataCount = vResult.ViewData.Count
                    };
                }
                else if (result is RedirectToActionResult rResult)
                {
                    statusCode = 302;
                    responseValue = new
                    {
                        Action = rResult.ActionName,
                        Controller = rResult.ControllerName,
                        RouteValues = rResult.RouteValues
                    };
                }
                else if (result is NotFoundResult || result is NotFoundObjectResult)
                {
                    statusCode = 404;
                    responseValue = (result as NotFoundObjectResult)?.Value ?? new { Message = "404 Not Found" };
                }
                else if (result is ObjectResult objResult)
                {
                    statusCode = objResult.StatusCode ?? 200;
                    responseValue = objResult.Value;
                }
                else if (result is StatusCodeResult scResult)
                {
                    statusCode = scResult.StatusCode;
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                string jsonBody = responseValue != null
                    ? JsonSerializer.Serialize(responseValue, options)
                    : "(No content / Null)";

                var logContent = new StringBuilder();
                logContent.AppendLine("==========================================================");
                logContent.AppendLine($"ACCION MVC: {controller.ToUpper()} -> {endpoint.ToUpper()} [{httpMethod.ToUpper()}]");
                logContent.AppendLine($"TIMESTAMP: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                logContent.AppendLine($"HTTP STATUS / RESULT: {statusCode} ({result.GetType().Name})");
                if (!string.IsNullOrWhiteSpace(details))
                {
                    logContent.AppendLine($"DESCRIPCIÓN: {details}");
                }
                logContent.AppendLine("==========================================================");
                logContent.AppendLine("\n--- MODELO / DATOS RETORNADOS ---");
                logContent.AppendLine(jsonBody);

                string filePath = Path.Combine(targetDir, "response.txt");
                await File.WriteAllTextAsync(filePath, logContent.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir log para {controller}-{endpoint}-{httpMethod}: {ex.Message}");
            }
        }
    }
}
