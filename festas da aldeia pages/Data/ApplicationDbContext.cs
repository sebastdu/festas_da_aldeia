using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace festas_da_aldeia_pages.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
    public DbSet<festas_da_aldeia.Models.Local> Local { get; set; } = default!;
    public DbSet<festas_da_aldeia.Models.Evento> Evento { get; set; } = default!;
    public DbSet<festas_da_aldeia.Models.Cartaz> Cartaz { get; set; } = default!;
    public DbSet<festas_da_aldeia.Models.Artista> Artista { get; set; } = default!;
    }
}
