using Microsoft.AspNetCore.Mvc.Routing;
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
    public class AuthorServiceTest : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly AuthorService authorService;
        private readonly AppDbContext context;
        private readonly Microsoft.Data.Sqlite.SqliteConnection connection;

        public AuthorServiceTest(ITestOutputHelper output)
        {
            this.output = output;

            connection = new Microsoft.Data.Sqlite.SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new AppDbContext(options);
            context.Database.EnsureCreated();
            authorService = new AuthorService(context);
        }

        public void Dispose()
        {
            context.Dispose();
            connection.Dispose();
        }

        [Fact]
        public async Task AddAuthor_ShouldAddAuthor()
        {
            var author = new Author { Name = "Author1", Surname = "Surname1" };
            var created = await authorService.AddAuthor(author);
            Assert.NotEqual(0, created.Id);
            Assert.Equal("Author1", created.Name);
        }

        [Fact]
        public async Task GetAuthors_ShouldReturnAllAuthors()
        {
            await authorService.AddAuthor(new Author { Name = "Author1", Surname = "Surname1" });
            await authorService.AddAuthor(new Author { Name = "Author2", Surname = "Surname2" });
            var authors = await authorService.GetAuthors();
            Assert.Equal(2, authors.Count);
            Assert.Equal("Author1", authors[0].Name);
            Assert.Equal("Author2", authors[1].Name);
        }

        [Fact]
        public async Task GetAuthorById_ShouldReturnAuthor()
        {
            var author = await authorService.AddAuthor(new Author { Name = "Author1", Surname = "Surname1" });
            var found = await authorService.GetAuthorById(author.Id);
            Assert.NotNull(found);
            Assert.Equal(author.Name, found!.Name);
        }

        [Fact]
        public async Task GetAuthorById_ShouldReturnNullIfNotFound()
        {
            var found = await authorService.GetAuthorById(999);
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteAuthor_ShouldRemoveAuthor()
        {
            var author = await authorService.AddAuthor(new Author { Name = "Author1", Surname = "Surname1" });
            await authorService.DeleteAuthor(author.Id);
            var found = await authorService.GetAuthorById(author.Id);
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteAuthor_ShouldThrowIfNotFound()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => authorService.DeleteAuthor(999));
        }
    }
}
