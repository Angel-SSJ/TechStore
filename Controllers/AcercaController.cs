using Microsoft.AspNetCore.Mvc;

namespace TechStore.Controllers
{
    public class AcercaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}