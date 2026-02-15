using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.service;
using MyApp.Domain;
using Serilog;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(IBookService bookService, ILogger<BookController> logger) : ControllerBase
    {
        private readonly IBookService bookService = bookService;

        [HttpGet("aaa/{id}")]
        public async Task<List<Book>> GetBooks(int id)
        {
            logger.LogInformation("Test logging {id}", id);
            var author = new Author { Id = id, Name = "", Surname = "" };
            return await bookService.FindBooks(author);
        }
    }
}
