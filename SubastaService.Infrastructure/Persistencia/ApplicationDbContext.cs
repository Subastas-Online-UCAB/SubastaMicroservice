using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SubastaService.Domain.Entidades;

namespace UsuarioServicio.Infrastructure.Persistencia
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Subasta> Subastas { get; set; }

        // Opcionalmente, para ver las tablas creadas, puedes sobreescribir OnModelCreating

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Opcional: Cambiar nombre de la tabla si quieres
            modelBuilder.Entity<Subasta>().ToTable("Subastas");

            // Opcional: Configuraciones de columnas si quieres afinar
            modelBuilder.Entity<Subasta>(entity =>
            {
               // entity.ToTable("Subastas");

                entity.HasKey(s => s.IdSubasta);

                entity.Property(s => s.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(s => s.Descripcion)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(s => s.PrecioBase)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(s => s.Duracion)
                    .IsRequired();

                entity.Property(s => s.CondicionParticipacion)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(s => s.FechaInicio)
                    .IsRequired();

                entity.Property(s => s.Estado)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.IncrementoMinimo)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(s => s.PrecioReserva)
                    .HasColumnType("decimal(18,2)");

                entity.Property(s => s.TipoSubasta)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.IdUsuario)
                    .IsRequired();

                entity.Property(s => s.IdProducto)
                    .IsRequired();
            });
        }
    }
}

