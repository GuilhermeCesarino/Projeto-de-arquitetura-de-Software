using Biblioteca.Usuarios.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Usuarios
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
