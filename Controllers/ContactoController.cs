using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class ContactoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactoViewModel());
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            TempData["MensajeEnviado"] = $"¡Gracias, {modelo.NombreCompleto}! Tu mensaje fue recibido correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}