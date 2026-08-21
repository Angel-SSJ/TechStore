using Microsoft.AspNetCore.Mvc;
using TechStore.Data;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        public IActionResult Index()
        {
            var categorias = TechStoreData.Categorias;
            return View(categorias);
        }

        // Muestra los productos de una categoría específica
        public IActionResult Productos(string nombre)
        {
            var productos = TechStoreData.Productos
                .Where(p => p.Categoria.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewData["CategoriaNombre"] = nombre;
            return View(productos);
        }
    }
}