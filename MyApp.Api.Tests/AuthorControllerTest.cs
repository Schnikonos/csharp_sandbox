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
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var mock = new Mock<IBookService>();
                mock.Setup(s => s.GetAuthors()).ReturnsAsync(new List<Author>
                {
                    new Author { Name = "Author 1", Surname = "Surname1" },
                    new Author { Name = "Author 2", Surname = "Surname2" }
                });
                services.AddSingleton(mock.Object);
            });
        }
    }

    public class AuthorControllerTest: IClassFixture<CustomFactory>
    {
        private readonly HttpClient _client;

        public AuthorControllerTest(CustomFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAuthors()
        {
            var authors = await _client.GetFromJsonAsync<List<Author>>("/api/author/all");
            Assert.Equal("Author 1", authors[0].Name);
        }
    }
}
