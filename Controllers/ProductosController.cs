using Microsoft.AspNetCore.Mvc;
using TechStore.Data;

namespace TechStore.Controllers
{
    public class ProductosController : Controller
    {
        public IActionResult Index()
        {
            var productos = TechStoreData.Productos;
            return View(productos);
        }

        public IActionResult Detalle(int id)
        {
            var producto = TechStoreData.Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }
    }
}