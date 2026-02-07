using MyApp.Domain;

namespace MyApp.Application.service
{
    public interface IBookService
    {
        void AddAuthor(Author author);
        List<Book> FindBooks(Author author);
        Task<List<Author>> GetAuthors();
    }
}