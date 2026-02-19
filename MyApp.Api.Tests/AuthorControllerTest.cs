using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyApp.Application.service;
using MyApp.Domain;
using Xunit;

namespace MyApp.Api.Tests
{
    public class CustomFactory : WebApplicationFactory<Program>
    {
        public Mock<IAuthorService> AuthorServiceMock { get; } = new Mock<IAuthorService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing IAuthorService registrations
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthorService));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddSingleton(AuthorServiceMock.Object);

                // Remove SchedulerService registration for tests
                var schedulerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SchedulerService));
                if (schedulerDescriptor != null)
                    services.Remove(schedulerDescriptor);
            });
        }
    }

    public class AuthorControllerTest : IClassFixture<CustomFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomFactory _factory;

        public AuthorControllerTest(CustomFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAuthors_ReturnsAllAuthors()
        {
            var expected = new List<Author>
            {
                new Author { Id = 1, Name = "Author 1", Surname = "Surname1" },
                new Author { Id = 2, Name = "Author 2", Surname = "Surname2" }
            };
            _factory.AuthorServiceMock.Reset();
            _factory.AuthorServiceMock.Setup(s => s.GetAuthors()).ReturnsAsync(expected);

            var response = await _client.GetAsync("/api/author/all");
            response.EnsureSuccessStatusCode();
            var authors = await response.Content.ReadFromJsonAsync<List<Author>>();
            Assert.NotNull(authors);
            Assert.Equal(2, authors.Count);
            Assert.Equal("Author 1", authors[0].Name);
        }

        [Fact]
        public async Task CreateAuthor_ReturnsCreated()
        {
            var author = new Author { Id = 10, Name = "New Author", Surname = "NewSurname" };
            _factory.AuthorServiceMock.Reset();
            _factory.AuthorServiceMock.Setup(s => s.AddAuthor(It.IsAny<Author>())).ReturnsAsync(author);

            var response = await _client.PostAsJsonAsync("/api/author", author);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Author>();
            Assert.NotNull(created);
            Assert.Equal(author.Name, created.Name);
        }

        [Fact]
        public async Task DeleteAuthor_ReturnsNoContent()
        {
            _factory.AuthorServiceMock.Reset();
            _factory.AuthorServiceMock.Setup(s => s.DeleteAuthor(1)).Returns(Task.CompletedTask);

            var response = await _client.DeleteAsync("/api/author/1");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuthor_NotFound()
        {
            _factory.AuthorServiceMock.Reset();
            _factory.AuthorServiceMock.Setup(s => s.DeleteAuthor(99)).ThrowsAsync(new KeyNotFoundException());

            var response = await _client.DeleteAsync("/api/author/99");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAuthorById_ReturnsAuthor()
        {
            var author = new Author { Id = 5, Name = "Author 5", Surname = "Surname5" };
            _factory.AuthorServiceMock.Reset();
            _factory.AuthorServiceMock.Setup(s => s.GetAuthorById(5)).ReturnsAsync(author);

            var response = await _client.GetAsync("/api/author/5");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Author>();
            Assert.NotNull(result);
            Assert.Equal(author.Name, result.Name);
        }

        [Fact]
        public async Task GetAuthorById_NotFound()
        {
            _factory.AuthorServiceMock.Reset();
            _factory.AuthorServiceMock.Setup(s => s.GetAuthorById(99)).ReturnsAsync((Author?)null);

            var response = await _client.GetAsync("/api/author/99");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
