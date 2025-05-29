using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UsuarioServicio.Infrastructure.Persistencia;

namespace ServicioSubasta.Infraestructura.Persistencia
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SubastasDb;Username=postgres;Password=161171");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

