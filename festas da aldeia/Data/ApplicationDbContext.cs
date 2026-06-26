using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;

namespace festas_da_aldeia.Data
{
    /// <summary>
    /// Contexto de acesso à base de dados da aplicação, estendendo o IdentityDbContext
    /// para integrar o ASP.NET Core Identity.
    /// Mapeia as entidades de negócio (Local, Evento, Artista e Cartaz) para tabelas relacionais.
    /// </summary>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        /// <summary>
        /// Tabela contendo os locais físicos ou recintos geográficos registados.
        /// </summary>
        public DbSet<Local> Locais { get; set; }

        /// <summary>
        /// Tabela contendo os eventos organizados.
        /// </summary>
        public DbSet<Evento> Eventos { get; set; }

        /// <summary>
        /// Tabela contendo os artistas e respetivas fichas informativas.
        /// </summary>
        public DbSet<Artista> Artistas { get; set; }

        /// <summary>
        /// Tabela contendo os agendamentos e escalas de atuações (Cartaz).
        /// </summary>
        public DbSet<Cartaz> Cartazes { get; set; }
    }
}
