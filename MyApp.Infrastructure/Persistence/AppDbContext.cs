using Microsoft.EntityFrameworkCore;
using MyApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();

        public override int SaveChanges() => base.SaveChanges();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired();
                entity.Property(a => a.Surname);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(b => b.Title).IsRequired();

                entity.HasOne(b => b.Author).WithMany(a => a.Books).HasForeignKey(b => b.AuthorId);

                entity.Property(u => u.Description);
            });

        }
    }
}
