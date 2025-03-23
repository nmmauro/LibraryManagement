using Bogus;
using LibraryManagementAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedData()
        {
            // Create roles if they don't exist
            await SeedRoles();

            // Create default users if they don't exist
            await SeedUsers();

            // Seed books if none exist
            if (!await _context.Books.AnyAsync())
            {
                // Generate 30 fake books
                var books = GenerateFakeBooks(30);
                await _context.Books.AddRangeAsync(books);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedRoles()
        {
            if (!await _roleManager.RoleExistsAsync("Librarian"))
                await _roleManager.CreateAsync(new IdentityRole("Librarian"));

            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
        }

        private async Task SeedUsers()
        {
            // Create a default librarian
            if (await _userManager.FindByEmailAsync("librarian@example.com") == null)
            {
                var librarian = new ApplicationUser
                {
                    UserName = "librarian",
                    Email = "librarian@example.com",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(librarian, "Password123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(librarian, "Librarian");
                }
            }

            // Create a default customer
            if (await _userManager.FindByEmailAsync("customer@example.com") == null)
            {
                var customer = new ApplicationUser
                {
                    UserName = "customer",
                    Email = "customer@example.com",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(customer, "Password123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(customer, "Customer");
                }
            }
        }

        private List<Book> GenerateFakeBooks(int count)
        {
            var categories = new[] { "Fiction", "Non-Fiction", "Science Fiction", "Fantasy", "Mystery", "Thriller", "Romance", "Biography", "History", "Science" };

            // Placeholder image URLs (you can customize these)
            var coverImages = new[]
            {
                "https://via.placeholder.com/300x450?text=Book+Cover",
                "https://via.placeholder.com/300x450/0000FF/FFFFFF?text=Book+Cover",
                "https://via.placeholder.com/300x450/FF0000/FFFFFF?text=Book+Cover",
                "https://via.placeholder.com/300x450/00FF00/000000?text=Book+Cover",
                "https://via.placeholder.com/300x450/FFFF00/000000?text=Book+Cover",
                "https://via.placeholder.com/300x450/FF00FF/FFFFFF?text=Book+Cover"
            };

            var faker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Commerce.ProductName())
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Description, f => f.Lorem.Paragraphs(3))
                .RuleFor(b => b.CoverImage, f => f.PickRandom(coverImages))
                .RuleFor(b => b.Publisher, f => f.Company.CompanyName())
                .RuleFor(b => b.PublicationDate, f => f.Date.Past(10))
                .RuleFor(b => b.Category, f => f.PickRandom(categories))
                .RuleFor(b => b.ISBN, f => f.Random.Replace("###-#-##-######-#"))
                .RuleFor(b => b.PageCount, f => f.Random.Number(100, 1000))
                .RuleFor(b => b.IsAvailable, true);

            return faker.Generate(count);
        }
    }
}