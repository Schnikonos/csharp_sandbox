using MyApp.Domain;

namespace MyApp.Application.service
{
    public interface IBookService
    {
        Task<Book> AddBook(Book book);
        Task<Book?> GetBookById(int id);
        Task<List<Book>> GetBooks();
        Task UpdateBook(Book book);
        Task DeleteBook(int id);
    }
}