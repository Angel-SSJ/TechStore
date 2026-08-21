using Microsoft.AspNetCore.Mvc;
using TechStore.Interfaces.Services;
using TechStore.Models;
using TechStore.Models.DTOs;

namespace TechStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var categories = includeInactive
                ? await _categoryService.GetAllAsync()
                : await _categoryService.GetAllActiveAsync();

            var result = categories.Select(MapToDetailDto).ToList();
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var categories = await _categoryService.GetAllActiveAsync();
            return Ok(categories.Select(MapToDetailDto).ToList());
        }

        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactive()
        {
            var categories = await _categoryService.GetAllInactiveAsync();
            return Ok(categories.Select(MapToDetailDto).ToList());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = $"Categoría con ID '{id}' no encontrada." });
            }

            return Ok(MapToDetailDto(category));
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var category = await _categoryService.GetByNameAsync(name);
            if (category == null)
            {
                return NotFound(new { message = $"Categoría con nombre '{name}' no encontrada." });
            }

            return Ok(MapToDetailDto(category));
        }

        [HttpGet("exists/{name}")]
        public async Task<IActionResult> ExistsByName(string name)
        {
            var exists = await _categoryService.ExistsByNameAsync(name);
            return Ok(new { name, exists });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var category = new Category
                {
                    Name = dto.Name.Trim()
                };

                var createdCategory = await _categoryService.AddAsync(category);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, MapToDetailDto(createdCategory));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = $"Categoría con ID '{id}' no encontrada." });
            }

            try
            {
                category.Name = dto.Name.Trim();
                var updatedCategory = await _categoryService.UpdateAsync(category);
                return Ok(MapToDetailDto(updatedCategory));
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
                var deletedCategory = await _categoryService.DeleteAsync(id);
                return Ok(new { message = "Categoría desactivada exitosamente.", category = MapToDetailDto(deletedCategory) });
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
                var restoredCategory = await _categoryService.RestoreAsync(id);
                return Ok(new { message = "Categoría restaurada exitosamente.", category = MapToDetailDto(restoredCategory) });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private static CategoryDetailDto MapToDetailDto(Category category)
        {
            return new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                ProductCount = category.CategoryProducts?.Count ?? 0,
                Products = category.CategoryProducts?
                    .Where(cp => cp.Product != null)
                    .Select(cp => new CategoryProductItemDto
                    {
                        Id = cp.Product.Id,
                        Name = cp.Product.Name,
                        Price = cp.Product.Price,
                        Stock = cp.Product.Stock,
                        PrimaryImageUrl = cp.Product.Images?.FirstOrDefault(i => i.IsPrimary)?.ImagePath
                            ?? cp.Product.Images?.FirstOrDefault()?.ImagePath
                    }).ToList() ?? new List<CategoryProductItemDto>()
            };
        }
    }
}
