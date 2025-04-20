using System.Diagnostics;
using CarWebSite.Models;
using Microsoft.AspNetCore.Mvc;
namespace CarWebSite.Controllers

{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
{
    var photos = new List<Photo>
    {
        new Photo { Url = "/Images/images.png" },
        new Photo { Url = "/Images/image.jpg" },
        new Photo { Url = "/Images/image(1).jpg" },
        new Photo { Url = "/Images/Mercedes-Logo.svg.png" },
        new Photo { Url = "/Images/emblem_001.jpg" }
    };
    // Pass the photos to the view
    return View(photos);
}

        public IActionResult Privacy()
        {
            return View();
        }
         public IActionResult NewCars()
        {
            return View();
        }

        public IActionResult UsedCars()
        {
            return View();
        }
        public IActionResult OfferYourCar()
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
