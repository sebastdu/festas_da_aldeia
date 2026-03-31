using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;

namespace festas_da_aldeia.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Local> Locais { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Cartaz> Cartazes { get; set; }

    }
}
