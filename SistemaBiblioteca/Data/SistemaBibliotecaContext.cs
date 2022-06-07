#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Models;

namespace SistemaBiblioteca.Data
{
    public class SistemaBibliotecaContext : DbContext
    {
        public SistemaBibliotecaContext (DbContextOptions<SistemaBibliotecaContext> options)
            : base(options)
        {
        }

        public DbSet<Material> Materiales { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Material>().ToTable("Material");
            modelBuilder.Entity<Autor>().ToTable("Autor");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Reserva>().ToTable("Reserva");
            modelBuilder.Entity<Prestamo>().ToTable("Prestamo");
        }

    }
}
