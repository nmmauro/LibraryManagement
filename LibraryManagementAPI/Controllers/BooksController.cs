using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementAPI.Data;
using LibraryManagementAPI.DTOs;
using LibraryManagementAPI.Models;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Reviews)
                .ToListAsync();

            return books.Select(book => new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                CoverImage = book.CoverImage,
                Publisher = book.Publisher,
                PublicationDate = book.PublicationDate,
                Category = book.Category,
                ISBN = book.ISBN,
                PageCount = book.PageCount,
                IsAvailable = book.IsAvailable,
                AverageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = book.Reviews.Count
            }).ToList();
        }

        // GET: api/Books/Featured
        [HttpGet("Featured")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetFeaturedBooks()
        {
            var books = await _context.Books
                .Include(b => b.Reviews)
                .ToListAsync();

            // Get 6 random books
            var random = new Random();
            var featuredBooks = books
                .OrderBy(x => random.Next())
                .Take(6)
                .Select(book => new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Description = book.Description,
                    CoverImage = book.CoverImage,
                    Publisher = book.Publisher,
                    PublicationDate = book.PublicationDate,
                    Category = book.Category,
                    ISBN = book.ISBN,
                    PageCount = book.PageCount,
                    IsAvailable = book.IsAvailable,
                    AverageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = book.Reviews.Count
                }).ToList();

            return featuredBooks;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailDTO>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .Include(b => b.CurrentCheckout)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts
                .Include(c => c.User)
                .Where(c => c.BookId == id && c.ReturnDate == null)
                .FirstOrDefaultAsync();

            var bookDetail = new BookDetailDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                CoverImage = book.CoverImage,
                Publisher = book.Publisher,
                PublicationDate = book.PublicationDate,
                Category = book.Category,
                ISBN = book.ISBN,
                PageCount = book.PageCount,
                IsAvailable = book.IsAvailable,
                AverageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = book.Reviews.Count,
                Reviews = book.Reviews.Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    Username = r.User.UserName
                }).ToList()
            };

            if (checkout != null)
            {
                bookDetail.CurrentCheckout = new CheckoutDTO
                {
                    Id = checkout.Id,
                    CheckoutDate = checkout.CheckoutDate,
                    DueDate = checkout.DueDate,
                    ReturnDate = checkout.ReturnDate,
                    Username = checkout.User.UserName
                };
            }

            return bookDetail;
        }

        // GET: api/Books/Search?query=harry
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> SearchBooks(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return await GetBooks();
            }

            var books = await _context.Books
                .Include(b => b.Reviews)
                .Where(b => b.Title.Contains(query) || b.Author.Contains(query))
                .ToListAsync();

            return books.Select(book => new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                CoverImage = book.CoverImage,
                Publisher = book.Publisher,
                PublicationDate = book.PublicationDate,
                Category = book.Category,
                ISBN = book.ISBN,
                PageCount = book.PageCount,
                IsAvailable = book.IsAvailable,
                AverageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = book.Reviews.Count
            }).ToList();
        }

        // POST: api/Books
        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<BookDTO>> CreateBook(BookCreateDTO bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Description = bookDto.Description,
                CoverImage = bookDto.CoverImage,
                Publisher = bookDto.Publisher,
                PublicationDate = bookDto.PublicationDate,
                Category = bookDto.Category,
                ISBN = bookDto.ISBN,
                PageCount = bookDto.PageCount,
                IsAvailable = true
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                CoverImage = book.CoverImage,
                Publisher = book.Publisher,
                PublicationDate = book.PublicationDate,
                Category = book.Category,
                ISBN = book.ISBN,
                PageCount = book.PageCount,
                IsAvailable = book.IsAvailable
            });
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> UpdateBook(int id, BookUpdateDTO bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Description = bookDto.Description;
            book.CoverImage = bookDto.CoverImage;
            book.Publisher = bookDto.Publisher;
            book.PublicationDate = bookDto.PublicationDate;
            book.Category = bookDto.Category;
            book.ISBN = bookDto.ISBN;
            book.PageCount = bookDto.PageCount;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Books/5/Checkout
        [HttpPost("{id}/Checkout")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CheckoutBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            if (!book.IsAvailable)
            {
                return BadRequest(new { message = "Book is already checked out" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var checkout = new Checkout
            {
                BookId = id,
                UserId = userId,
                CheckoutDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            _context.Checkouts.Add(checkout);
            book.IsAvailable = false;
            book.CurrentCheckout = checkout;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Books/5/Return
        [HttpPost("{id}/Return")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            if (book.IsAvailable)
            {
                return BadRequest(new { message = "Book is not checked out" });
            }

            var checkout = await _context.Checkouts
                .Where(c => c.BookId == id && c.ReturnDate == null)
                .FirstOrDefaultAsync();

            if (checkout == null)
            {
                return NotFound();
            }

            checkout.ReturnDate = DateTime.UtcNow;
            book.IsAvailable = true;
            book.CurrentCheckout = null; // Clear the current checkout reference

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}