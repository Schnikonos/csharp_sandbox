using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyApp.Application.service;
using MyApp.Domain;
using MyApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace MyApp.Application.Tests
{
    public class BookServiceTest : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly BookService bookService;
        private readonly AuthorService authorService;
        private readonly AppDbContext context;

        public BookServiceTest(ITestOutputHelper output)
        {
            this.output = output;

            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new AppDbContext(options);
            context.Database.EnsureCreated(); // Ensure schema is created
            bookService = new BookService(context);
            authorService = new AuthorService(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public async Task AddBook_ShouldAddBook()
        {
            var author = new Author { Name = "John", Surname = "Doe" };
            await authorService.AddAuthor(author);

            var book = new Book { Title = "Test Book", AuthorId = author.Id, Description = "Desc" };
            var result = await bookService.AddBook(book);

            Assert.NotEqual(0, result.Id);
            Assert.Equal("Test Book", result.Title);
            Assert.Equal(author.Id, result.AuthorId);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnBookWithAuthor()
        {
            var author = new Author { Name = "Jane", Surname = "Smith" };
            await authorService.AddAuthor(author);

            var book = new Book { Title = "Book 1", AuthorId = author.Id, Description = "Desc" };
            await bookService.AddBook(book);

            var fetched = await bookService.GetBookById(book.Id);

            Assert.NotNull(fetched);
            Assert.Equal("Book 1", fetched!.Title);
            Assert.NotNull(fetched.Author);
            Assert.Equal("Jane", fetched.Author.Name);
        }

        [Fact]
        public async Task GetBooks_ShouldReturnAllBooksWithAuthors()
        {
            var author = new Author { Name = "A", Surname = "B" };
            await authorService.AddAuthor(author);

            await bookService.AddBook(new Book { Title = "Book A", AuthorId = author.Id, Description = "D1" });
            await bookService.AddBook(new Book { Title = "Book B", AuthorId = author.Id, Description = "D2" });

            var books = await bookService.GetBooks();

            Assert.Equal(2, books.Count);
            Assert.All(books, b => Assert.NotNull(b.Author));
        }

        [Fact]
        public async Task UpdateBook_ShouldModifyBook()
        {
            var author = new Author { Name = "C", Surname = "D" };
            await authorService.AddAuthor(author);

            var book = new Book { Title = "Old Title", AuthorId = author.Id, Description = "Old" };
            await bookService.AddBook(book);

            book.Title = "New Title";
            book.Description = "New Desc";
            await bookService.UpdateBook(book);

            var updated = await bookService.GetBookById(book.Id);
            Assert.Equal("New Title", updated!.Title);
            Assert.Equal("New Desc", updated.Description);
        }


        [Fact]
        public async Task GetAuthors_ShouldReturnOrderedAuthors()
        {
            await authorService.AddAuthor(new Author { Name = "Zoe", Surname = "Alpha" });
            await authorService.AddAuthor(new Author { Name = "Anna", Surname = "Beta" });

            var authors = await authorService.GetAuthors();

            Assert.Equal(2, authors.Count);
            Assert.Equal("Anna", authors[0].Name);
            Assert.Equal("Zoe", authors[1].Name);
        }
    }
}
