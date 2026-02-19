using Microsoft.Extensions.Logging;
using MyApp.Domain;
using MyApp.Infrastructure.Persistence;
using MyApp.Templating;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public interface IAuthorInfoService
    {
        Task<Object> PrepareMessage(CancellationToken stoppingToken);
    }

    public class AuthorInfoService(AppDbContext db, HtmlTemplateRenderer renderer, ILogger<AuthorInfoService> logger) : IAuthorInfoService
    {
        public async Task<Object> PrepareMessage(CancellationToken stoppingToken)
        {
            db.Database.EnsureCreated();
            List<Author> authors = [.. db.Authors];
            var html = await renderer.RenderAsync("Templates.AuthorInfo.cshtml", authors);
            logger.LogInformation("Test {0}", html);
            return new();
        }
    }
}
