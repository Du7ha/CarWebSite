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
                new Photo { Url = "/Images/images.png", Description = "BMW" },
                new Photo { Url = "/Images/image.jpg", Description = "Audi" },
                new Photo { Url = "/Images/image(1).jpg", Description = "Toyota" },
                new Photo { Url = "/Images/Mercedes-Logo.svg.png", Description = "Mercedes" },
                new Photo { Url = "/Images/emblem_001.jpg", Description = "Ferrari" }
            };

            var categories = new List<Category>
            {
                new Category { Name = "Sedan", Description = "Sedan" },
                new Category { Name = "SUV", Description = "SUV" },
                new Category { Name = "Truck", Description = "Truck" },
                new Category { Name = "Electric", Description = "Electric" },
                new Category { Name = "Luxury", Description = "Luxury" },
                new Category { Name = "Coupe", Description = "Coupe" },
                new Category { Name = "Crossover", Description = "Crossover" },
                new Category { Name = "Minivan", Description = "Minivan" },
                new Category { Name = "Convertible", Description = "Convertible" },
                new Category { Name = "Hatchback", Description = "Hatchback" },
                new Category { Name = "Hybrid", Description = "Hybrid" }
            };

            var viewModel = new HomeViewModel
            {
                Photos = photos,
                Categories = categories
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NewCars(string bodyType = null, string brand = null, string priceRange = null, string color = null)
        {
            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
            ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
            
            // Store the filter values in ViewBag to pre-select them in the UI
            ViewBag.SelectedBodyType = bodyType;
            ViewBag.SelectedBrand = brand;
            ViewBag.SelectedPriceRange = priceRange;
            ViewBag.SelectedColor = color;
            
            // In a real app, these filters would be used to query the database
            // For now, we'll just pass them to the view
            
            return View();
        }

        public IActionResult UsedCars(string bodyType = null, string brand = null, string priceRange = null, string color = null)
        {
            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
            ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
            
            // Store the filter values in ViewBag
            ViewBag.SelectedBodyType = bodyType;
            ViewBag.SelectedBrand = brand;
            ViewBag.SelectedPriceRange = priceRange;
            ViewBag.SelectedColor = color;
            
            return View();
        }
        
        public IActionResult CarDetails()
        {
            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
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
        // ----------------------- Added SignUp -----------------------

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string ConfirmPassword)
        {
            if (user.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
            }

            if (ModelState.IsValid)
            {
                // Save the user in your database here (example)
                // _dbContext.Users.Add(user);
                // _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]

        
        public IActionResult ToggleFavorite(int carId)
        {
            return Json(new { success = true, message = "Toggled favorite status" });
        }
    }
}