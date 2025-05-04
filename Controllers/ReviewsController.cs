using System.Security.Claims;
using CarWebSite.Data;
using CarWebSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWebSite.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly CarWebSiteContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(
            CarWebSiteContext context,
            ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Client)
                .ThenInclude(c => c.User)
                .ToListAsync();

            var viewModel = new ReviewsViewModel
            {
                Reviews = reviews
            };

            return View("~/Views/Home/Reviews.cshtml", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(ReviewFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid for review submission");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError($"Property: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                var reviews = await _context.Reviews
                    .Include(r => r.Client)
                    .ThenInclude(c => c.User)
                    .ToListAsync();

                var reviewsViewModel = new ReviewsViewModel
                {
                    Reviews = reviews,
                    ReviewForm = viewModel // Preserve the form data
                };

                return View("~/Views/Home/Reviews.cshtml", reviewsViewModel);
            }

            try
            {
                // Get the current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    _logger.LogError($"User with ID {userId} not found");
                    return NotFound("User not found");
                }

                // Get the ClientId
                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.UserID == userId);

                if (client == null)
                {
                    _logger.LogError($"Client with UserID {userId} not found");
                    return NotFound("Client not found");
                }

                // Create a new Review entity from the view model
                var review = new Review
                {
                    Title = viewModel.Title,
                    Rating = viewModel.Rating,
                    ReviewContent = viewModel.ReviewContent,
                    UserId = userId,
                    Author = user.UserName,
                    Date = DateTime.UtcNow,
                    ClientId = client.Id
                };

                // Add to database
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Review with ID {review.Id} successfully added");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting review");
                ModelState.AddModelError("", "An error occurred while submitting your review. Please try again.");

                var reviews = await _context.Reviews
                    .Include(r => r.Client)
                    .ThenInclude(c => c.User)
                    .ToListAsync();

                var reviewsViewModel = new ReviewsViewModel
                {
                    Reviews = reviews,
                    ReviewForm = viewModel // Preserve the form data
                };

                return View("~/Views/Home/Reviews.cshtml", reviewsViewModel);
            }
        }
    }
}