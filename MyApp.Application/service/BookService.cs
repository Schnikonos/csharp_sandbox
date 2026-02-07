using MyApp.Domain;
using MyApp.IdGenerator.ClassicEvents;
using MyApp.Infrastructure.Persistence;
using MyApp.Templating;
using Serilog;
using System;

namespace MyApp.Application.service
{
    public class BookService(IAppDbContext appDbContext) : IBookService
    {
        public List<Book> FindBooks(Author author)
        {
            List<Book> books = [.. appDbContext.Books];
            var book = new Book { Title = "Title1", Description = "Description1", Author = author, AuthorId = author.Id };
            var list = new List<Book>();
            list.Add(book);
            return list;
        }

        public void AddAuthor(Author author)
        {
            appDbContext.Authors.Add(author);
            appDbContext.SaveChanges();
        }

        async public Task<List<Author>> GetAuthors()
        {
            return [.. appDbContext.Authors];
        }
    }
}
