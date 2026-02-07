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
    public class BookServiceTest
    {
        private readonly ITestOutputHelper output;
        private readonly BookService bookService;
        private readonly Mock<IAppDbContext> dbMock;

        public BookServiceTest(ITestOutputHelper output)
        {
            this.output = output;
            dbMock = new Mock<IAppDbContext>();
            bookService = new BookService(dbMock.Object);
        }

        [Fact]
        public async Task TestMockDbGet()
        {
            output.WriteLine("test1");
            
            var data = new List<Author>
            {
                new Author { Id = 1, Name = "Author1", Surname = "Surname1"  },
                new Author { Id = 2, Name = "Author2", Surname = "Surname2"  },
            }.AsQueryable();
            var mockSet = new Mock<DbSet<Author>>();
            mockSet.As<IQueryable<Author>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Author>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Author>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Author>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            dbMock.Setup(x => x.Authors).Returns(mockSet.Object);
            List<Author> res = await bookService.GetAuthors();

            output.WriteLine("test1 {0}", res[0].Name);
            Assert.Equal("Author1", res[0].Name);
            dbMock.Verify(x => x.Authors, Times.Once);
        }
    }
}
