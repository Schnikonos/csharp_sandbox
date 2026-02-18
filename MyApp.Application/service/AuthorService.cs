using Microsoft.EntityFrameworkCore;
using MyApp.Domain;
using MyApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public class AuthorService(AppDbContext appDbContext) : IAuthorService
    {
        // --- Author methods
        public async Task AddAuthor(Author author)
        {
            await appDbContext.Authors.AddAsync(author);
            await appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAuthor(int id)
        {
            int rowsAffected = await appDbContext.Authors
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                throw new KeyNotFoundException($"Author with ID {id} not found.");
            }
        }

        async public Task<List<Author>> GetAuthors()
        {
            return await appDbContext.Authors
                .OrderBy(a => a.Name).ThenBy(a => a.Surname)
                .ToListAsync();
        }
    }
}
