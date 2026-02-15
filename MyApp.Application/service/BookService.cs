using Microsoft.EntityFrameworkCore;
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
        async public Task<List<Book>> FindBooks(Author author)
        {
            List<Book> books = await appDbContext.Books.ToListAsync();
            var book = new Book { Title = "Title1", Description = "Description1", Author = author, AuthorId = author.Id };
            var list = new List<Book>();
            list.Add(book);
            return list;
        }

        public async Task AddAuthor(Author author)
        {
            await appDbContext.Authors.AddAsync(author);
            appDbContext.SaveChanges();
        }

        async public Task<List<Author>> GetAuthors()
        {
            return await appDbContext.Authors.ToListAsync();
        }
    }
}
