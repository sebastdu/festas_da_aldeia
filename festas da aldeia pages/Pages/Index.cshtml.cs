using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;

namespace festas_da_aldeia_pages.Pages
{
    public class MapEventDto
    {
        public int IdEvento { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string DataInicio { get; set; } = string.Empty;
        public string DataFim { get; set; } = string.Empty;
        public int IdLocal { get; set; }
        public string LocalNome { get; set; } = string.Empty;
        public string Coordenadas { get; set; } = string.Empty;
        public bool IsOutside { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Evento> EventosProximos { get; set; } = [];
        public IList<Local> LocaisPrincipais { get; set; } = [];
        public IList<MapEventDto> EventosMapa { get; set; } = [];
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

            // Obter eventos para o mapa com coordenadas de formato latitude,longitude válidas
            var eventosComCoordenadas = await _context.Eventos
                .Include(e => e.Local)
                .Where(e => e.Local != null && !string.IsNullOrEmpty(e.Local.Coordenadas))
                .ToListAsync();

            EventosMapa = eventosComCoordenadas
                .Where(e => !e.Local.Coordenadas!.Trim().StartsWith("http"))
                .Select(e => new MapEventDto
                {
                    IdEvento = e.IdEvento,
                    Nome = e.Nome,
                    Descricao = e.Descricao ?? string.Empty,
                    DataInicio = e.DataInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    DataFim = e.DataFim.ToString("yyyy-MM-ddTHH:mm:ss"),
                    IdLocal = e.IdLocal,
                    LocalNome = e.Local.Nome,
                    Coordenadas = e.Local.Coordenadas?.Trim() ?? string.Empty,
                    IsOutside = e.Local.Outside
                })
                .ToList();
        }
    }
}
