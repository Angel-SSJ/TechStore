using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var productosDestacados = TechStoreData.ProductosDestacados;
            return View(productosDestacados);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}