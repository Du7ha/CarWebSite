using System.Security.Claims;
using CarWebSite.Data;
using CarWebSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWebSite.Controllers
{
    public class CarController : Controller
    {
        private readonly CarWebSiteContext _context;
        private readonly ILogger<CarController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarController(CarWebSiteContext context, ILogger<CarController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string bodyType, string brand, string color, string priceRange, int page = 1)
        {
            var carsQuery = _context.Cars.AsQueryable();

            // تطبيق الفلاتر بناءً على المدخلات
            if (!string.IsNullOrEmpty(bodyType)) carsQuery = carsQuery.Where(c => c.BodyType == bodyType);
            if (!string.IsNullOrEmpty(brand)) carsQuery = carsQuery.Where(c => c.Brand == brand);
            if (!string.IsNullOrEmpty(color)) carsQuery = carsQuery.Where(c => c.Color == color);
            if (!string.IsNullOrEmpty(priceRange))
            {
                var prices = priceRange.Split('-');
                var minPrice = int.Parse(prices[0]);
                var maxPrice = int.Parse(prices[1]);
                carsQuery = carsQuery.Where(c => c.Price >= minPrice && c.Price <= maxPrice);
            }

            int pageSize = 12; // عدد السيارات في كل صفحة
            int totalCars = carsQuery.Count();
            var cars = carsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // حساب عدد الصفحات
            int totalPages = (int)Math.Ceiling((double)totalCars / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.BodyTypes = _context.Cars.Select(c => c.BodyType).Distinct().ToList();
            ViewBag.Brands = _context.Cars.Select(c => c.Brand).Distinct().ToList();
            ViewBag.Colors = _context.Cars.Select(c => c.Color).Distinct().ToList();
            ViewBag.SelectedBodyType = bodyType;
            ViewBag.SelectedBrand = brand;
            ViewBag.SelectedColor = color;
            ViewBag.SelectedPriceRange = priceRange;

            return View(cars);
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(string bodyType = null, string brand = null, string priceRange = null, string color = null)
        {
            var query = _context.Cars.Where(c => !c.IsSold);

            // Apply filters if provided
            if (!string.IsNullOrEmpty(bodyType))
            {
                query = query.Where(c => c.BodyType == bodyType);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(c => c.Brand == brand);
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(c => c.Color == color);
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                // Parse price range (format: min-max)
                var parts = priceRange.Split('-');
                if (parts.Length == 2 && decimal.TryParse(parts[0], out decimal min) && decimal.TryParse(parts[1], out decimal max))
                {
                    query = query.Where(c => c.Price >= min && c.Price <= max);
                }
            }

            var cars = await query
                .Include(c => c.Photos)
                .Include(c => c.Seller)
                .Include(c => c.SavedByUsers)
                .ToListAsync();

            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
            ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
            
            ViewBag.SelectedBodyType = bodyType;
            ViewBag.SelectedBrand = brand;
            ViewBag.SelectedPriceRange = priceRange;
            ViewBag.SelectedColor = color;

            return View(cars);
        }

        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Photos)
                .Include(c => c.Seller)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            bool isFavorite = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                isFavorite = await _context.SavedCars.AnyAsync(sc => sc.UserId == userId && sc.CarId == id);
            }

            ViewBag.IsFavorite = isFavorite;

            return View(car);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToFavorite(int carId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            // Check if car exists
            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
            {
                return NotFound();
            }

            // Check if already saved
            var existingSavedCar = await _context.SavedCars
                .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.CarId == carId);

            if (existingSavedCar != null)
            {
                // Already in favorites, remove it
                _context.SavedCars.Remove(existingSavedCar);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = false, message = "Removed from favorites" });
            }
            else
            {
                // Add to favorites
                var savedCar = new SavedCar
                {
                    UserId = userId,
                    CarId = carId,
                    SavedDate = DateTime.UtcNow
                };

                _context.SavedCars.Add(savedCar);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true, message = "Added to favorites" });
            }
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorite(int carId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var savedCar = await _context.SavedCars.FirstOrDefaultAsync(sc => sc.UserId == userId && sc.CarId == carId);

            if (savedCar == null)
            {
                return Json(new { success = false, message = "Favorite not found" });
            }

            _context.SavedCars.Remove(savedCar);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Removed from favorites" });
        }


        [Authorize]
        [HttpGet]
        public IActionResult Offer()
        {
            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
            ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
            
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Offer(Car car, IFormFile mainImage)
        {
            

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            car.UserId = userId;
            car.ListingDate = DateTime.UtcNow;
            car.IsSold = false;

            // Process main image
            if (mainImage != null)
            {
                string uniqueFileName = await SaveImage(mainImage);
                car.ImagePath = uniqueFileName;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
                ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
                ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
                return View(car);
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            
            

            return RedirectToAction("Details", new { id = car.CarId });
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyListings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var listings = await _context.Cars
                .Include(c => c.Photos)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(listings);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = await _context.SavedCars
                .Include(sc => sc.Car)
                    .ThenInclude(c => c.Photos)
                .Include(sc => sc.Car)
                    .ThenInclude(c => c.Seller)
                .Where(sc => sc.UserId == userId)
                .ToListAsync();

            return View(favorites);
        }
    }
}