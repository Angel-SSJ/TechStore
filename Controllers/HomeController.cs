using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechStore.Interfaces;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductQueries _productQueries;

        public HomeController(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productQueries.GetAllAsync();

            var featuredProducts = products
                .Where(product => product.Featured && product.IsActive)
                .ToList();

            return View(featuredProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}