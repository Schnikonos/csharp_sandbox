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


        [HttpGet("all")]
        async public Task<List<Author>> GetAuthors()
        {
            logger.LogInformation("GetAuthors");
            return await this._authorService.GetAuthors();
        }

        [HttpPost]
        public async Task AddAuthor(Author author)
        {
            logger.LogInformation("AddAuthor {author}", author);
            await this._authorService.AddAuthor(author);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAuthor(int id)
        {
            logger.LogInformation("Delete Author {id}", id);
            await this._authorService.DeleteAuthor(id);
        }
    }
}
