using Biblioteca.Livros.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Livros
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Livro> Livros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Livro>().HasKey(l => l.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
