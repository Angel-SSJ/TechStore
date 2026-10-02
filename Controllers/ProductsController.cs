using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechStore.DTOs;
using TechStore.Interfaces;
using TechStore.Models;
using TechStore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductQueries _productService;
        private readonly IProductApplicationService _productApplicationService;
        private readonly ICategoryQueries _categoryService;
        private readonly IProductLifecycle _productLifecycle;

        public ProductsController(
            IProductQueries productService,
            IProductApplicationService productApplicationService,
            ICategoryQueries categoryService,
            IProductLifecycle productLifecycle)
        {
            _productService = productService;
            _productApplicationService = productApplicationService;
            _categoryService = categoryService;
            _productLifecycle = productLifecycle;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _productService.GetByIdWithDetailsAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllActiveAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInput product, List<Guid>? selectedCategoryIds, ICollection<IFormFile>? images)
        {
            if (!ModelState.IsValid)
            {
                await PopulateProductOptionsAsync(selectedCategoryIds);
                return View(new Product(product.Name, product.Description, product.Price, product.Stock, product.Featured, product.IsActive));
            }

            try
            {
                await _productApplicationService.CreateAsync(product, selectedCategoryIds, images);
                TempData["Success"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(selectedCategoryIds), ex.Message);
                await PopulateProductOptionsAsync(selectedCategoryIds);
                return View(new Product(product.Name, product.Description, product.Price, product.Stock, product.Featured, product.IsActive));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Producto creado, pero ocurrió un error con las imágenes: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productService.GetByIdWithDetailsAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _categoryService.GetAllActiveAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductInput product, List<Guid>? selectedCategoryIds, ICollection<IFormFile>? images)
        {
            var targetId = id != Guid.Empty ? id : product.Id;
            if (targetId == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                var updatedProduct = await _productApplicationService.UpdateAsync(
                    targetId,
                    product,
                    selectedCategoryIds ?? new List<Guid>(),
                    images);
                if (updatedProduct == null)
                {
                    return NotFound();
                }

                TempData["Success"] = images != null && images.Count > 0
                    ? $"Producto e imágenes ({images.Count}) actualizados correctamente."
                    : "Producto actualizado correctamente.";
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(selectedCategoryIds), ex.Message);
                var existingProduct = await _productService.GetByIdWithDetailsAsync(targetId);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.UpdateDetails(product.Name, product.Description, product.Price, product.Stock, product.Featured);
                ViewBag.SelectedCategoryIds = selectedCategoryIds ?? new List<Guid>();
                await PopulateProductOptionsAsync(selectedCategoryIds);
                return View(existingProduct);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Información del producto guardada, pero hubo un error con las imágenes: " + ex.Message;
            }

            return RedirectToAction(nameof(Edit), new
            {
                id = targetId
            });
        }

        private async Task PopulateProductOptionsAsync(IEnumerable<Guid>? selectedCategoryIds = null)
        {
            ViewBag.Categories = await _categoryService.GetAllActiveAsync();
            ViewBag.SelectedCategoryIds = selectedCategoryIds ?? Enumerable.Empty<Guid>();
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _productLifecycle.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Restore(Guid id)
        {
            await _productLifecycle.RestoreAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImages(Guid id, ICollection<IFormFile> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                {
                    TempData["Error"] = "Debes seleccionar al menos una imagen.";
                    return RedirectToAction(nameof(Edit), new
                    {
                        id
                    });
                }

                await _productApplicationService.AddImagesAsync(id, images);
                TempData["Success"] = $"Se han subido {images.Count} imagen(es) correctamente.";
                return RedirectToAction(nameof(Edit), new
                {
                    id
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al subir imágenes: {ex.Message}";
                return RedirectToAction(nameof(Edit), new
                {
                    id
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage(Guid imageId, Guid productId)
        {
            try
            {
                await _productApplicationService.RemoveImageAsync(imageId);
                TempData["Success"] = "Imagen eliminada correctamente.";
                return RedirectToAction(nameof(Edit), new
                {
                    id = productId
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar imagen: {ex.Message}";
                return RedirectToAction(nameof(Edit), new
                {
                    id = productId
                });
            }
        }
    }
}
