using System.Collections.Generic;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyApp.Application.service;
using MyApp.Domain;
using Xunit;

namespace MyApp.Api.Tests
{
    public class BookCustomFactory : WebApplicationFactory<Program>
    {
        public Mock<IBookService> BookServiceMock { get; } = new Mock<IBookService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBookService));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddSingleton(BookServiceMock.Object);

                // Remove SchedulerService registration for tests
                var schedulerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SchedulerService));
                if (schedulerDescriptor != null)
                    services.Remove(schedulerDescriptor);
            });
        }
    }

    public class BookControllerTest : IClassFixture<BookCustomFactory>
    {
        private readonly HttpClient _client;
        private readonly BookCustomFactory _factory;

        public BookControllerTest(BookCustomFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetBooks_ReturnsAllBooks()
        {
            var expected = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", AuthorId = 1, Description = "Desc1" },
                new Book { Id = 2, Title = "Book 2", AuthorId = 2, Description = "Desc2" }
            };
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.GetBooks()).ReturnsAsync(expected);

            var response = await _client.GetAsync("/api/book");
            response.EnsureSuccessStatusCode();
            var books = await response.Content.ReadFromJsonAsync<List<Book>>();
            Assert.NotNull(books);
            Assert.Equal(2, books.Count);
            Assert.Equal("Book 1", books[0].Title);
        }

        [Fact]
        public async Task GetBookById_ReturnsBook()
        {
            var book = new Book { Id = 5, Title = "Book 5", AuthorId = 1, Description = "Desc5" };
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.GetBookById(5)).ReturnsAsync(book);

            var response = await _client.GetAsync("/api/book/5");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(result);
            Assert.Equal(book.Title, result.Title);
        }

        [Fact]
        public async Task GetBookById_NotFound()
        {
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.GetBookById(99)).ReturnsAsync((Book?)null);

            var response = await _client.GetAsync("/api/book/99");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateBook_ReturnsCreated()
        {
            var book = new Book { Id = 10, Title = "New Book", AuthorId = 1, Description = "Desc" };
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.AddBook(It.IsAny<Book>())).ReturnsAsync(book);

            var response = await _client.PostAsJsonAsync("/api/book", book);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(created);
            Assert.Equal(book.Title, created.Title);
        }

        [Fact]
        public async Task UpdateBook_ReturnsNoContent()
        {
            var book = new Book { Id = 1, Title = "Updated", AuthorId = 1, Description = "Desc" };
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.UpdateBook(It.IsAny<Book>())).Returns(Task.CompletedTask);

            var response = await _client.PutAsJsonAsync("/api/book/1", book);
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBook_BadRequestOnIdMismatch()
        {
            var book = new Book { Id = 2, Title = "Updated", AuthorId = 1, Description = "Desc" };
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.UpdateBook(It.IsAny<Book>())).Returns(Task.CompletedTask);

            var response = await _client.PutAsJsonAsync("/api/book/1", book);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent()
        {
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.DeleteBook(1)).Returns(Task.CompletedTask);

            var response = await _client.DeleteAsync("/api/book/1");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBook_NotFound()
        {
            _factory.BookServiceMock.Reset();
            _factory.BookServiceMock.Setup(s => s.DeleteBook(99)).ThrowsAsync(new KeyNotFoundException());

            var response = await _client.DeleteAsync("/api/book/99");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
