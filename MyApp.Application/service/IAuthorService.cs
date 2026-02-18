using MyApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public interface IAuthorService
    {
        Task AddAuthor(Author author);
        Task<List<Author>> GetAuthors();
        Task DeleteAuthor(int id);
    }
}
