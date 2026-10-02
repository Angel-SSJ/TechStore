using TechStore.DTOs;
using TechStore.Interfaces;
using TechStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace TechStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryQueries _categoryQueries;
        private readonly ICategoryApplicationService _categoryApplication;
        private readonly ICategoryLifecycle _categoryLifecycle;
        private readonly IProductQueries _productQueries;

        public CategoryController(
            ICategoryQueries categoryQueries,
            ICategoryApplicationService categoryApplication,
            ICategoryLifecycle categoryLifecycle,
            IProductQueries productQueries)
        {
            _categoryQueries = categoryQueries;
            _categoryApplication = categoryApplication;
            _categoryLifecycle = categoryLifecycle;
            _productQueries = productQueries;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _categoryQueries.GetAllAsync());
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var category = await _categoryQueries.GetByIdWithProductsAsync(id);
            return category == null ? NotFound() : View(category);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _productQueries.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryInput category, List<Guid>? selectedProductIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _productQueries.GetAllAsync();
                return View(new Category(category.Name, category.Description, category.IsActive));
            }

            try
            {
                var createdCategory = await _categoryApplication.CreateAsync(category, selectedProductIds);
                TempData["Success"] = "Categoría creada correctamente.";
                return RedirectToAction(nameof(Edit), new
                {
                    id = createdCategory.Id
                });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(category.Name), ex.Message);
                ViewBag.Products = await _productQueries.GetAllAsync();
                return View(new Category(category.Name, category.Description, category.IsActive));
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryQueries.GetByIdWithProductsAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _productQueries.GetAllAsync();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryInput category, List<Guid>? selectedProductIds)
        {
            if (!ModelState.IsValid)
            {
                var invalidCategory = await _categoryQueries.GetByIdWithProductsAsync(id);
                if (invalidCategory == null)
                {
                    return NotFound();
                }

                invalidCategory.UpdateDetails(category.Name, category.Description);
                ViewBag.Products = await _productQueries.GetAllAsync();
                return View(invalidCategory);
            }

            try
            {
                var updatedCategory = await _categoryApplication.UpdateAsync(id, category, selectedProductIds ?? new List<Guid>());
                if (updatedCategory == null)
                {
                    return NotFound();
                }

                TempData["Success"] = "Categoría actualizada correctamente.";
                return RedirectToAction(nameof(Edit), new
                {
                    id
                });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(category.Name), ex.Message);
                var invalidCategory = await _categoryQueries.GetByIdWithProductsAsync(id);
                if (invalidCategory == null)
                {
                    return NotFound();
                }

                invalidCategory.UpdateDetails(category.Name, category.Description);
                ViewBag.Products = await _productQueries.GetAllAsync();
                return View(invalidCategory);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            if (!await _categoryLifecycle.DeleteAsync(id))
            {
                TempData["Error"] = "No se puede desactivar una categoría relacionada con productos.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            await _categoryLifecycle.RestoreAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
