using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.service;
using MyApp.Domain;
using Serilog;
using System.Text.Json;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(IBookService bookService, ILogger<BookController> logger) : ControllerBase
    {
        private readonly IBookService _bookService = bookService;
        private readonly ILogger<BookController> _logger = logger;

        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            _logger.LogInformation("GetBooks");
            var books = await _bookService.GetBooks();
            return Ok(books);
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            _logger.LogInformation("GetBook - {}", id);
            var book = await _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
                
            return Ok(book);
        }

        // POST: api/Book
        [HttpPost]
        public async Task<ActionResult<Book>> AddBook([FromBody] Book book)
        {
            _logger.LogInformation("AddBook {}", JsonSerializer.Serialize(book));
            var created = await _bookService.AddBook(book);
            return CreatedAtAction(nameof(GetBook), new { id = created.Id }, created);
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            _logger.LogInformation("UpdateBook {} {}", id, JsonSerializer.Serialize(book));
            if (id != book.Id)
                return BadRequest("ID mismatch");

            await _bookService.UpdateBook(book);
            return NoContent();
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation("DeleteBook {}", id);
            try
            {
                await _bookService.DeleteBook(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
