using Microsoft.AspNetCore.Mvc;
using TechStore.Interfaces.Services;
using TechStore.Models;
using TechStore.Models.DTOs;

namespace TechStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: /Category
        [HttpGet]
        public async Task<IActionResult> Index(bool includeInactive = false)
        {
            var categories = includeInactive
                ? await _categoryService.GetAllAsync()
                : await _categoryService.GetAllActiveAsync();

            var result = categories.Select(MapToDetailDto).ToList();
            ViewBag.IncludeInactive = includeInactive;
            return View(result);
        }

        // GET: /Category/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = $"Categoría con ID '{id}' no encontrada.";
                return NotFound();
            }

            return View(MapToDetailDto(category));
        }

        // GET: /Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryDto());
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var category = new Category
                {
                    Name = dto.Name.Trim()
                };

                var createdCategory = await _categoryService.AddAsync(category);
                TempData["SuccessMessage"] = $"Categoría '{createdCategory.Name}' creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(dto);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // GET: /Category/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = $"Categoría con ID '{id}' no encontrada.";
                return NotFound();
            }

            var dto = new UpdateCategoryDto
            {
                Name = category.Name
            };

            ViewBag.CategoryId = id;
            ViewBag.CategoryName = category.Name;
            return View(dto);
        }

        // POST: /Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(dto);
            }

            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = $"Categoría con ID '{id}' no encontrada.";
                return NotFound();
            }

            try
            {
                category.Name = dto.Name.Trim();
                await _categoryService.UpdateAsync(category);
                TempData["SuccessMessage"] = $"Categoría '{dto.Name}' actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CategoryId = id;
                return View(dto);
            }
        }

        // GET: /Category/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = $"Categoría con ID '{id}' no encontrada.";
                return NotFound();
            }

            return View(MapToDetailDto(category));
        }

        // POST: /Category/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var deletedCategory = await _categoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Categoría '{deletedCategory.Name}' desactivada exitosamente.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Category/Restore/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var restoredCategory = await _categoryService.RestoreAsync(id);
                TempData["SuccessMessage"] = $"Categoría '{restoredCategory.Name}' restaurada exitosamente.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
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
