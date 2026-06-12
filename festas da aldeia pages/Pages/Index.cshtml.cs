using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;

namespace festas_da_aldeia_pages.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Evento> EventosProximos { get; set; } = [];
        public IList<Local> LocaisPrincipais { get; set; } = [];
        public int TotalArtistas { get; set; }
        public int TotalEventos { get; set; }
        public int TotalLocais { get; set; }

        public async Task OnGetAsync()
        {
            // Obter os próximos 3 eventos
            EventosProximos = await _context.Eventos
                .Include(e => e.Local)
                .OrderBy(e => e.DataInicio)
                .Take(3)
                .ToListAsync();

            // Obter os 3 primeiros locais registados
            LocaisPrincipais = await _context.Locais
                .Take(3)
                .ToListAsync();

            // Contagens para estatísticas rápidas
            TotalArtistas = await _context.Artistas.CountAsync();
            TotalEventos = await _context.Eventos.CountAsync();
            TotalLocais = await _context.Locais.CountAsync();
        }
    }
}
