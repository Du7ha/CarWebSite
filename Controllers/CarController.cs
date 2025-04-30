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
                    .ThenInclude(s => s.User) // Ensure Seller and nested User are included
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            bool isFavorite = false;

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Retrieve the Client Id from Clients table
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);

                if (client != null)
                {
                    isFavorite = await _context.SavedCars
                        .AnyAsync(sc => sc.ClientId == client.Id && sc.CarId == id);
                }
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the Client corresponding to the current user
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                return Json(new { success = false, message = "Client profile not found." });
            }

            // Check if car exists
            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
            {
                return NotFound();
            }

            // Check if already saved
            var existingSavedCar = await _context.SavedCars
                .FirstOrDefaultAsync(sc => sc.ClientId == client.Id && sc.CarId == carId);

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
                    ClientId = client.Id,
                    CarId = carId,
                    SavedDate = DateTime.UtcNow
                };

                _context.SavedCars.Add(savedCar);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true, message = "Added to favorites" });
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                return Forbid();
            }

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == id && c.SellerId == client.Id);
            if (car == null)
            {
                return NotFound();
            }

            ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
            ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
            ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };

            return View(car);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Car updatedCar, IFormFile mainImage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                return Forbid();
            }

            var existingCar = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == updatedCar.CarId && c.SellerId == client.Id);
            if (existingCar == null)
            {
                return NotFound();
            }

            // تحديث القيم
            existingCar.Brand = updatedCar.Brand;
            existingCar.Model = updatedCar.Model;
            existingCar.Year = updatedCar.Year;
            existingCar.Price = updatedCar.Price;
            existingCar.Color = updatedCar.Color;
            existingCar.BodyType = updatedCar.BodyType;
            existingCar.Mileage = updatedCar.Mileage;

            // إذا كانت هناك صورة جديدة تم رفعها
            if (mainImage != null)
            {
                // حذف الصورة القديمة من السيرفر
                if (!string.IsNullOrEmpty(existingCar.ImagePath))
                {
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, existingCar.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                // حفظ الصورة الجديدة
                string uniqueFileName = await SaveImage(mainImage);
                existingCar.ImagePath = uniqueFileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("MyListings");
        }
        // دالة واحدة فقط لتحميل الصورة وحفظها
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
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorite(int carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get client ID from user ID
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                return Json(new { success = false, message = "Client not found" });
            }

            // Remove the saved car
            var savedCar = await _context.SavedCars
                .FirstOrDefaultAsync(sc => sc.ClientId == client.Id && sc.CarId == carId);

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the Client ID for the current user
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                return BadRequest("Client profile not found.");
            }

            // Assign the seller (Client) ID to the car
            car.SellerId = client.Id;
            car.ListingDate = DateTime.UtcNow;
            car.IsSold = false;


            // Enforce mileage based on role
            if (User.IsInRole("Admin"))
            {
                car.Mileage = 0;
            }
            else if (User.IsInRole("Client"))
            {
                
                if (car.Mileage < 5000)
                {
                    ModelState.AddModelError("Mileage", "Clients can only offer cars with mileage more than or equal to 5000.");
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.BodyTypes = Enum.GetNames(typeof(BodyType));
                ViewBag.Brands = new List<string> { "Ford", "Toyota", "Honda", "BMW", "Mercedes", "Audi", "Chevrolet", "Nissan", "Porsche", "Rolls-Royce", "Mahindra" };
                ViewBag.Colors = new List<string> { "Black", "White", "Silver", "Red", "Blue", "Green", "Yellow", "Gray" };
                return View(car);
            }
            // Process main image
            if (mainImage != null)
            {
                string uniqueFileName = await SaveImage(mainImage);
                car.ImagePath = uniqueFileName;
            }
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = car.CarId });
        }


  

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the client for the current user
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                return View(new List<SavedCar>()); // or redirect with a message
            }

            // Get all saved cars for this client
            var favorites = await _context.SavedCars
                .Where(sc => sc.ClientId == client.Id)
                .Include(sc => sc.Car)
                    .ThenInclude(c => c.Photos)
                .Include(sc => sc.Car)
                    .ThenInclude(c => c.Seller)
                        .ThenInclude(s => s.User) // include user info of seller
                .ToListAsync();

            return View(favorites);
        }



        // Add this action to your CarController
        [HttpGet]
        public IActionResult FilterCars(string bodyType, string brand, string priceRange, string color)
        {
            // Start with all cars
            var cars = _context.Cars.Where(c => !c.IsSold).AsQueryable();

            // Apply body type filter
            if (!string.IsNullOrEmpty(bodyType))
            {
                cars = cars.Where(c => c.BodyType == bodyType);
            }

            // Apply brand filter
            if (!string.IsNullOrEmpty(brand))
            {
                cars = cars.Where(c => c.Brand == brand);
            }

            // Apply color filter
            if (!string.IsNullOrEmpty(color))
            {
                cars = cars.Where(c => c.Color == color);
            }

            // Apply price range filter
            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "Under $30,000":
                        cars = cars.Where(c => c.Price < 30000);
                        break;
                    case "$30,000 - $50,000":
                        cars = cars.Where(c => c.Price >= 30000 && c.Price <= 50000);
                        break;
                    case "$50,000 - $100,000":
                        cars = cars.Where(c => c.Price > 50000 && c.Price <= 100000);
                        break;
                    case "Over $100,000":
                        cars = cars.Where(c => c.Price > 100000);
                        break;
                }
            }

            // For AJAX partial view rendering
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CarListings", cars.ToList());
            }

            return View(cars.ToList());
        }


        // Add this action to your CarController
        [HttpGet]
        public IActionResult FilterNewCars(string bodyType, string brand, string priceRange, string color)
        {
            // Start with all new cars (mileage = 0)
            var cars = _context.Cars.Where(c => !c.IsSold && c.Mileage == 0).AsQueryable();

            // Apply body type filter
            if (!string.IsNullOrEmpty(bodyType))
            {
                cars = cars.Where(c => c.BodyType == bodyType);
            }

            // Apply brand filter
            if (!string.IsNullOrEmpty(brand))
            {
                cars = cars.Where(c => c.Brand == brand);
            }

            // Apply color filter
            if (!string.IsNullOrEmpty(color))
            {
                cars = cars.Where(c => c.Color == color);
            }

            // Apply price range filter
            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "Under $30,000":
                        cars = cars.Where(c => c.Price < 30000);
                        break;
                    case "$30,000 - $50,000":
                        cars = cars.Where(c => c.Price >= 30000 && c.Price <= 50000);
                        break;
                    case "$50,000 - $100,000":
                        cars = cars.Where(c => c.Price > 50000 && c.Price <= 100000);
                        break;
                    case "Over $100,000":
                        cars = cars.Where(c => c.Price > 100000);
                        break;
                }
            }

            // For AJAX partial view rendering
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_NewCarListings", cars.ToList());
            }

            return View(cars.ToList());
        }
        
        [Authorize]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the client ID from user
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                return Forbid();
            }

            // Find the car owned by this client
            var car = await _context.Cars
                .Include(c => c.SavedByUsers)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            // Remove any saved favorites
            if (car.SavedByUsers != null)
            {
                _context.SavedCars.RemoveRange(car.SavedByUsers);
            }

            // Optionally delete image file from disk
            if (!string.IsNullOrEmpty(car.ImagePath))
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, car.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return RedirectToAction("index","Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyListings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get Client ID
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
                return Forbid();

            var listings = await _context.Cars
                .Where(c => c.SellerId == client.Id)
                .ToListAsync();

            return View(listings);
        }



    }
}