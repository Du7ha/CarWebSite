using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
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
        new Photo { 
            Url = "/Images/images.png", 
            Description = "BMW" 
        },
        new Photo { 
            Url = "/Images/image.jpg", 
            Description = "Audi" 
        },
        new Photo { 
            Url = "/Images/image(1).jpg", 
            Description = "Toyota" 
        },
        new Photo { 
            Url = "/Images/Mercedes-Logo.svg.png", 
            Description = "Mercedes" 
        },
        new Photo { 
            Url = "/Images/emblem_001.jpg", 
            Description = "Ferrari" 
        }
    };

    var categories = new List<Category>
    {
        new Category { 
            Name = "sedan", 
            Description = "Sedan" 
        },
        new Category { 
            Name = "suv", 
            Description = "SUV" 
        },
        new Category { 
            Name = "truck", 
            Description = "Truck" 
        },
        new Category { 
            Name = "electric", 
            Description = "Electric" 
        },
        new Category { 
            Name = "luxury", 
            Description = "Luxury" 
        },
        new Category { 
            Name = "Coupe", 
            Description = "Coupe" 
        },
        new Category { 
            Name = "crossover", 
            Description = "crossover" 
        },
        new Category { 
            Name = "Minivan", 
            Description = "Minivan" 
        },
        new Category { 
            Name = "convertible", 
            Description = "convertible" 
        },
        new Category { 
            Name = "hatchback", 
            Description = "hatchback" 
        },
        new Category { 
            Name = "Hybrid", 
            Description = "Hybrid" 
        }
    };

    // Combine both lists into a ViewModel
    var viewModel = new HomeViewModel
    {
        Photos = photos,
        Categories = categories
    };

    // Pass the ViewModel to the view
    return View(viewModel);
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
         public IActionResult Profile()
        {
            return View();
        }
        public IActionResult About()
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
