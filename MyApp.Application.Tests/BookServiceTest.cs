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
    public class BookServiceTest : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly BookService bookService;
        private readonly AppDbContext context;

        public BookServiceTest(ITestOutputHelper output)
        {
            this.output = output;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            context = new AppDbContext(options);
            context.Database.EnsureDeleted(); // avoid data persistence between tests
            bookService = new BookService(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public async Task TestMockDbGet()
        {
            output.WriteLine("test1");

            context.Authors.Add(new Author { Id = 1, Name = "Author1", Surname = "Surname1" });
            context.Authors.Add(new Author { Id = 2, Name = "Author2", Surname = "Surname2" });
            context.SaveChanges();

            List<Author> res = await bookService.GetAuthors();

            output.WriteLine("test1 {0}", res[0].Name);
            Assert.Equal("Author1", res[0].Name);
            //dbMock.Verify(x => x.Authors, Times.Once);
        }
    }
}
