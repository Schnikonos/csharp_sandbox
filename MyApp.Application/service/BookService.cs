using Microsoft.EntityFrameworkCore;
using MyApp.Domain;
using MyApp.IdGenerator.ClassicEvents;
using MyApp.Infrastructure.Persistence;
using MyApp.Templating;
using Serilog;
using System;

namespace MyApp.Application.service
{
    public class BookService(AppDbContext appDbContext) : IBookService
    {
        // --- Book CRUD ---

        // Create
        public async Task<Book> AddBook(Book book)
        {
            await appDbContext.Books.AddAsync(book);
            await appDbContext.SaveChangesAsync();
            return book;
        }

        // Read (single)
        public async Task<Book?> GetBookById(int id)
        {
            return await appDbContext.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // Read (all, ordered)
        public async Task<List<Book>> GetBooks()
        {
            return await appDbContext.Books
                .Include(b => b.Author)
                .OrderBy(b => b.Title)
                .ThenBy(b => b.Id)
                .ToListAsync();
        }

        // Update
        public async Task UpdateBook(Book book)
        {
            appDbContext.Books.Update(book);
            await appDbContext.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteBook(int id)
        {
            int rowsAffected = await appDbContext.Books
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }
        }
    }
}
