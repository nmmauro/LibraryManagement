using LibraryManagementAPI.Controllers;
using LibraryManagementAPI.Data;
using LibraryManagementAPI.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibraryManagementAPI.Tests
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task GetBooks_ReturnsAllBooks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb_" + Guid.NewGuid().ToString())
                .Options;

            // Create test data
            using (var context = new ApplicationDbContext(options))
            {
                // Create test users for reviews
                var user = new ApplicationUser
                {
                    Id = "test-user-id",
                    UserName = "testuser"
                };
                context.Users.Add(user);

                // Add test books
                var book1 = new Book
                {
                    Id = 1,
                    Title = "Test Book 1",
                    Author = "Test Author 1",
                    Description = "Test Description 1",
                    CoverImage = "https://placehold.co/300x450/darkgray/white?text=Test+Book+1",
                    Publisher = "Test Publisher",
                    PublicationDate = new DateTime(2023, 1, 1),
                    Category = "Fiction",
                    ISBN = "1234567890",
                    PageCount = 200,
                    IsAvailable = true,
                    Reviews = new List<Review>()
                };

                var book2 = new Book
                {
                    Id = 2,
                    Title = "Test Book 2",
                    Author = "Test Author 2",
                    Description = "Test Description 2",
                    CoverImage = "https://placehold.co/300x450/darkgray/white?text=Test+Book+2",
                    Publisher = "Test Publisher",
                    PublicationDate = new DateTime(2023, 2, 1),
                    Category = "Non-Fiction",
                    ISBN = "0987654321",
                    PageCount = 300,
                    IsAvailable = true,
                    Reviews = new List<Review>()
                };

                context.Books.Add(book1);
                context.Books.Add(book2);
                await context.SaveChangesAsync();
            }

            // Act and Assert
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new BooksController(context);
                var result = await controller.GetBooks();

                // Verify the result
                Assert.NotNull(result.Value);
                var books = result.Value;
                Assert.Equal(2, books.Count());

                // Verify book 1
                var book1 = books.FirstOrDefault(b => b.Id == 1);
                Assert.NotNull(book1);
                Assert.Equal("Test Book 1", book1.Title);
                Assert.Equal("Test Author 1", book1.Author);

                // Verify book 2
                var book2 = books.FirstOrDefault(b => b.Id == 2);
                Assert.NotNull(book2);
                Assert.Equal("Test Book 2", book2.Title);
                Assert.Equal("Test Author 2", book2.Author);
            }
        }

        [Fact]
        public async Task GetBook_WithValidId_ReturnsBook()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb_GetBook_" + Guid.NewGuid().ToString())
                .Options;

            int testBookId = 1;

            // Create test data
            using (var context = new ApplicationDbContext(options))
            {
                // Create test user for reviews
                var user = new ApplicationUser
                {
                    Id = "test-user-id",
                    UserName = "testuser"
                };
                context.Users.Add(user);

                // Add test book
                var book = new Book
                {
                    Id = testBookId,
                    Title = "Test Book",
                    Author = "Test Author",
                    Description = "Test Description",
                    CoverImage = "https://placehold.co/300x450/darkgray/white?text=Test+Book",
                    Publisher = "Test Publisher",
                    PublicationDate = new DateTime(2023, 1, 1),
                    Category = "Fiction",
                    ISBN = "1234567890",
                    PageCount = 200,
                    IsAvailable = true,
                    Reviews = new List<Review>()
                };

                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act and Assert
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new BooksController(context);
                var result = await controller.GetBook(testBookId);

                // Check the result is not NotFound
                Assert.IsNotType<NotFoundResult>(result.Result);

                // Check the book details
                var bookDetail = result.Value;
                Assert.NotNull(bookDetail);
                Assert.Equal("Test Book", bookDetail.Title);
                Assert.Equal("Test Author", bookDetail.Author);
                Assert.Equal("Fiction", bookDetail.Category);
                Assert.Equal(200, bookDetail.PageCount);
            }
        }

        [Fact]
        public async Task GetBook_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb_NotFound_" + Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.GetBook(999); // Non-existent ID

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }
    }
}