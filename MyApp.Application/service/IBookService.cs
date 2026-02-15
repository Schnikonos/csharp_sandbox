using MyApp.Domain;

namespace MyApp.Application.service
{
    public interface IBookService
    {
        Task AddAuthor(Author author);
        Task<List<Book>> FindBooks(Author author);
        Task<List<Author>> GetAuthors();
    }
}