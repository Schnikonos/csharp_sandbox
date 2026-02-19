using MyApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public interface IAuthorService
    {
        Task<Author> AddAuthor(Author author);
        Task<List<Author>> GetAuthors();
        Task<Author?> GetAuthorById(int id);
        Task DeleteAuthor(int id);
    }
}
