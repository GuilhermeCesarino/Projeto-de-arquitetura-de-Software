using Biblioteca.Emprestimos.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Emprestimos
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Emprestimo> Emprestimos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Emprestimo>().HasKey(e => e.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
