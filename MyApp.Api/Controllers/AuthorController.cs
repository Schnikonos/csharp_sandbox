using Microsoft.AspNetCore.Mvc;
using MyApp.Application.service;
using MyApp.Domain;
using Serilog;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(IAuthorService authorService, ILogger<AuthorController> logger) : ControllerBase
    {
        private readonly IAuthorService _authorService = authorService;
        private readonly ILogger<AuthorController> _logger = logger;


        [HttpGet("all")]
        async public Task<List<Author>> GetAuthors()
        {
            _logger.LogInformation("GetAuthors");
            return await this._authorService.GetAuthors();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthorById(int id)
        {
            _logger.LogInformation("GetAuthorById - {}", id);
            var author = await _authorService.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<Author>> AddAuthor(Author author)
        {
            _logger.LogInformation("AddAuthor {author}", author);
            Author created = await this._authorService.AddAuthor(author);
            return CreatedAtAction(nameof(GetAuthorById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            _logger.LogInformation("Delete Author {id}", id);
            try
            {
                await this._authorService.DeleteAuthor(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
