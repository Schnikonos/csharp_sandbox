using Microsoft.AspNetCore.Mvc;
using MyApp.Application.service;
using MyApp.Domain;
using Serilog;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(BookService bookService, ILogger<AuthorController> logger) : ControllerBase
    {
        private readonly BookService bookService = bookService;


        [HttpGet("all")]
        async public Task<List<Author>> GetAuthors()
        {
            logger.LogInformation("GetAuthors");
            return await this.bookService.GetAuthors();
        }

        [HttpPost]
        public void AddAuthor(Author author)
        {
            logger.LogInformation("AddAuthor {author}", author);
            this.bookService.AddAuthor(author);
        }
    }
}
