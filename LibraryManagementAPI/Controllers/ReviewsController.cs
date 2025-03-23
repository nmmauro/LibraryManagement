using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementAPI.Data;
using LibraryManagementAPI.DTOs;
using LibraryManagementAPI.Models;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/books/{bookId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/books/5/reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            return reviews.Select(r => new ReviewDTO
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                Username = r.User.UserName
            }).ToList();
        }

        // POST: api/books/5/reviews
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ReviewDTO>> CreateReview(int bookId, ReviewCreateDTO reviewDto)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null)
            {
                return NotFound();
            }

            // Fix: Get the userId from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID could not be determined" });
            }

            // Check if user already reviewed this book
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);

            if (existingReview != null)
            {
                // Update existing review
                existingReview.Rating = reviewDto.Rating;
                existingReview.Comment = reviewDto.Comment;
                existingReview.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new review
                var review = new Review
                {
                    BookId = bookId,
                    UserId = userId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
            }

            await _context.SaveChangesAsync();

            // Get the user name for the response
            var user = await _context.Users.FindAsync(userId);

            return new ReviewDTO
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow,
                Username = user?.UserName ?? "Unknown"
            };
        }
    }
}