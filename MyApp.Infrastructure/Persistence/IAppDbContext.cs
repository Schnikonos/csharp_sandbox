using Microsoft.EntityFrameworkCore;
using MyApp.Domain;

namespace MyApp.Infrastructure.Persistence
{
    // interface is used to allow mocking in the unittest
    public interface IAppDbContext
    {
        DbSet<Author> Authors { get; }
        DbSet<Book> Books { get; }
        int SaveChanges();
    }
}