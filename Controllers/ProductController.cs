using Microsoft.AspNetCore.Mvc;
using TechStore.Interfaces.Services;
using TechStore.Models;
using TechStore.Models.DTOs;

namespace TechStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
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

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] bool includeInactive = false)
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

            var result = products.Select(MapToDetailDto).ToList();
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var products = await _productService.GetAllActiveAsync();
            return Ok(products.Select(MapToDetailDto).ToList());
        }

        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactive()
        {
            var products = await _productService.GetAllInactiveAsync();
            return Ok(products.Select(MapToDetailDto).ToList());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Producto con ID '{id}' no encontrado." });
            }

            return Ok(MapToDetailDto(product));
        }

        [HttpGet("by-category/{categoryId:guid}")]
        public async Task<IActionResult> GetByCategoryId(Guid categoryId)
        {
            var products = await _productService.GetByCategoryIdAsync(categoryId);
            return Ok(products.Select(MapToDetailDto).ToList());
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string term)
        {
            var products = await _productService.SearchByNameAsync(term);
            return Ok(products.Select(MapToDetailDto).ToList());
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 5)
        {
            var products = await _productService.GetLowStockAsync(threshold);
            return Ok(products.Select(MapToDetailDto).ToList());
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var productId = Guid.NewGuid();

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

                // Guardar producto primero
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

                // Recargar producto con categorías e imágenes
                var fullProduct = await _productService.GetByIdAsync(createdProduct.Id);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, MapToDetailDto(fullProduct ?? createdProduct));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Producto con ID '{id}' no encontrado." });
            }

            try
            {
                product.Name = dto.Name.Trim();
                product.Description = dto.Description?.Trim() ?? string.Empty;
                product.Price = dto.Price;
                product.Stock = dto.Stock;

                await _productService.UpdateAsync(product);

                // Actualizar categorías si fueron provistas
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

                var updatedProduct = await _productService.GetByIdAsync(id);
                return Ok(MapToDetailDto(updatedProduct ?? product));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deletedProduct = await _productService.DeleteAsync(id);
                return Ok(new { message = "Producto desactivado exitosamente.", product = MapToDetailDto(deletedProduct) });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id:guid}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var restoredProduct = await _productService.RestoreAsync(id);
                var fullProduct = await _productService.GetByIdAsync(id);
                return Ok(new { message = "Producto restaurado exitosamente.", product = MapToDetailDto(fullProduct ?? restoredProduct) });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ==========================================
        // Manejo de Imágenes
        // ==========================================

        [HttpPost("{id:guid}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImages(Guid id, [FromForm] AddProductImagesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Producto con ID '{id}' no encontrado." });
            }

            try
            {
                var addedImages = new List<ProductImageDto>();
                foreach (var file in dto.Images.Where(f => f.Length > 0))
                {
                    var productImage = await _productImageStorageService.SaveProductImageAsync(id, file);
                    await _productService.AddImageToProductAsync(id, productImage);
                    addedImages.Add(MapToImageDto(productImage));
                }

                return Ok(new { message = "Imágenes agregadas exitosamente.", images = addedImages });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}/images/{imageId:guid}")]
        public async Task<IActionResult> DeleteImage(Guid id, Guid imageId)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Producto con ID '{id}' no encontrado." });
            }

            var removedImage = await _productService.RemoveImageFromProductAsync(id, imageId);
            if (removedImage == null)
            {
                return NotFound(new { message = $"Imagen con ID '{imageId}' no encontrada en este producto." });
            }

            // Eliminar archivo físico
            _productImageStorageService.DeleteProductImage(removedImage);

            return Ok(new { message = "Imagen eliminada exitosamente.", imageId });
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
