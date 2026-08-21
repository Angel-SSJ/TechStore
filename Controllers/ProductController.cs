using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Interfaces.Services;
using TechStore.Models;
using TechStore.Models.DTOs;

namespace TechStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductImageStorageService _productImageStorageService;
        private readonly ICategoryService _categoryService;

        public ProductController(
            IProductService productService,
            IProductImageStorageService productImageStorageService,
            ICategoryService categoryService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productImageStorageService = productImageStorageService ?? throw new ArgumentNullException(nameof(productImageStorageService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: /Product
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm = null,
            Guid? categoryId = null,
            bool includeInactive = false)
        {
            IList<Product> products;

            if (!string.IsNullOrWhiteSpace(searchTerm) || (categoryId.HasValue && categoryId.Value != Guid.Empty))
            {
                products = await _productService.FilterAsync(searchTerm, categoryId);
            }
            else if (includeInactive)
            {
                products = await _productService.GetAllAsync();
            }
            else
            {
                products = await _productService.GetAllActiveAsync();
            }

            var categories = await _categoryService.GetAllActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.IncludeInactive = includeInactive;

            var result = products.Select(MapToDetailDto).ToList();
            return View(result);
        }

        // GET: /Product/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = $"Producto con ID '{id}' no encontrado.";
                return NotFound();
            }

            return View(MapToDetailDto(product));
        }

        // GET: /Product/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesViewBag();
            return View(new CreateProductDto());
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesViewBag(dto.CategoryIds);
                return View(dto);
            }

            try
            {
                var product = new Product
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim() ?? string.Empty,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Asignar categorías
                if (dto.CategoryIds != null && dto.CategoryIds.Any())
                {
                    foreach (var categoryId in dto.CategoryIds.Distinct())
                    {
                        product.Categories.Add(new CategoryProduct
                        {
                            ProductId = product.Id,
                            CategoryId = categoryId
                        });
                    }
                }

                // Guardar producto
                var createdProduct = await _productService.AddAsync(product);

                // Guardar imágenes si se proporcionaron
                if (dto.Images != null && dto.Images.Any())
                {
                    foreach (var imageFile in dto.Images.Where(f => f.Length > 0))
                    {
                        var productImage = await _productImageStorageService.SaveProductImageAsync(createdProduct.Id, imageFile);
                        await _productService.AddImageToProductAsync(createdProduct.Id, productImage);
                    }
                }

                TempData["SuccessMessage"] = $"Producto '{createdProduct.Name}' creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateCategoriesViewBag(dto.CategoryIds);
                return View(dto);
            }
        }

        // GET: /Product/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = $"Producto con ID '{id}' no encontrado.";
                return NotFound();
            }

            var dto = new UpdateProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryIds = product.Categories.Select(c => c.CategoryId).ToList()
            };

            ViewBag.ProductId = product.Id;
            ViewBag.ExistingImages = product.Images.OrderBy(i => i.ImageNumber).Select(MapToImageDto).ToList();
            await PopulateCategoriesViewBag(dto.CategoryIds);
            return View(dto);
        }

        // POST: /Product/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var existingProd = await _productService.GetByIdAsync(id);
                ViewBag.ProductId = id;
                ViewBag.ExistingImages = existingProd?.Images.OrderBy(i => i.ImageNumber).Select(MapToImageDto).ToList() ?? new List<ProductImageDto>();
                await PopulateCategoriesViewBag(dto.CategoryIds);
                return View(dto);
            }

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = $"Producto con ID '{id}' no encontrado.";
                return NotFound();
            }

            try
            {
                product.Name = dto.Name.Trim();
                product.Description = dto.Description?.Trim() ?? string.Empty;
                product.Price = dto.Price;
                product.Stock = dto.Stock;

                await _productService.UpdateAsync(product);

                // Actualizar categorías
                if (dto.CategoryIds != null)
                {
                    await _productService.UpdateProductCategoriesAsync(id, dto.CategoryIds);
                }

                // Guardar nuevas imágenes si se subieron
                if (dto.NewImages != null && dto.NewImages.Any())
                {
                    foreach (var imageFile in dto.NewImages.Where(f => f.Length > 0))
                    {
                        var productImage = await _productImageStorageService.SaveProductImageAsync(id, imageFile);
                        await _productService.AddImageToProductAsync(id, productImage);
                    }
                }

                TempData["SuccessMessage"] = $"Producto '{dto.Name}' actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.ProductId = id;
                ViewBag.ExistingImages = product.Images.OrderBy(i => i.ImageNumber).Select(MapToImageDto).ToList();
                await PopulateCategoriesViewBag(dto.CategoryIds);
                return View(dto);
            }
        }

        // GET: /Product/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = $"Producto con ID '{id}' no encontrado.";
                return NotFound();
            }

            return View(MapToDetailDto(product));
        }

        // POST: /Product/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var deletedProduct = await _productService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Producto '{deletedProduct.Name}' desactivado exitosamente.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Product/Restore/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var restoredProduct = await _productService.RestoreAsync(id);
                TempData["SuccessMessage"] = $"Producto '{restoredProduct.Name}' restaurado exitosamente.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Product/{id}/DeleteImage/{imageId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(Guid id, Guid imageId)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = $"Producto con ID '{id}' no encontrado.";
                return NotFound();
            }

            var removedImage = await _productService.RemoveImageFromProductAsync(id, imageId);
            if (removedImage == null)
            {
                TempData["ErrorMessage"] = "Imagen no encontrada en este producto.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            // Eliminar archivo físico
            _productImageStorageService.DeleteProductImage(removedImage);

            TempData["SuccessMessage"] = "Imagen eliminada exitosamente.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        // GET: /Product/LowStock
        [HttpGet]
        public async Task<IActionResult> LowStock(int threshold = 5)
        {
            var products = await _productService.GetLowStockAsync(threshold);
            ViewBag.Threshold = threshold;
            return View(products.Select(MapToDetailDto).ToList());
        }

        private async Task PopulateCategoriesViewBag(IEnumerable<Guid>? selectedCategoryIds = null)
        {
            var categories = await _categoryService.GetAllActiveAsync();
            var selectedSet = selectedCategoryIds?.ToHashSet() ?? new HashSet<Guid>();
            ViewBag.CategoryOptions = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = selectedSet.Contains(c.Id)
            }).ToList();
        }

        private static ProductDetailDto MapToDetailDto(Product product)
        {
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Categories = product.Categories?
                    .Where(cp => cp.Category != null)
                    .Select(cp => new ProductCategoryDto
                    {
                        Id = cp.Category.Id,
                        Name = cp.Category.Name
                    }).ToList() ?? new List<ProductCategoryDto>(),
                Images = product.Images?
                    .OrderBy(i => i.ImageNumber)
                    .Select(MapToImageDto)
                    .ToList() ?? new List<ProductImageDto>()
            };
        }

        private static ProductImageDto MapToImageDto(ProductImage image)
        {
            return new ProductImageDto
            {
                Id = image.Id,
                ImagePath = image.ImagePath,
                ImageNumber = image.ImageNumber,
                OriginalFileName = image.OriginalFileName,
                FileSize = image.FileSize,
                IsPrimary = image.IsPrimary,
                FormattedFileSize = image.FormattedFileSize
            };
        }
    }
}
