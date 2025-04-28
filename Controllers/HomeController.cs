using System.Diagnostics;
using System.Security.Claims;
using CarWebSite.Data;
using CarWebSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CarWebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarWebSiteContext _context;
        public HomeController(ILogger<HomeController> logger, CarWebSiteContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var photos = new List<Photo>
            {
                new Photo { Url = "/Images/images.png", Description = "Kia" },
                new Photo { Url = "/Images/image.jpg", Description = "BMW" },
                new Photo { Url = "/Images/image(1).jpg", Description = "Audi" },
                new Photo { Url = "/Images/Mercedes-Logo.svg.png", Description = "Mercedes" },
                new Photo { Url = "/Images/emblem_001.jpg", Description = "Toyota" },
                new Photo { Url = "/Images/Nissan.png", Description = "Nissan" },
                new Photo { Url = "/Images/Ford_Logo.png", Description = "Ford" },
                new Photo { Url = "/Images/honda.png", Description = "Honda" },
                new Photo { Url = "/Images/Mahindra.png", Description = "Mahindra" },
                new Photo { Url = "/Images/porsche.jpg", Description = "Porsche" }
            };


            var viewModel = new HomeViewModel
            {
                Photos = photos,
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NewCars()
        {

            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
            ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
            
            
            return View(_context.Cars.ToList());
        }

        public IActionResult UsedCars()
        {
            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
            ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
            
            return View(_context.Cars.ToList());
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

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.OrdersAsBuyer)
                    .ThenInclude(o => o.Car)
                .Include(u => u.SavedCars)
                    .ThenInclude(sc => sc.Car)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
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