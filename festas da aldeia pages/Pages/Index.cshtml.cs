using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;

namespace festas_da_aldeia_pages.Pages
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) concebido para representar as informações mínimas
    /// necessárias para desenhar e apresentar marcadores de eventos no mapa interativo (Leaflet).
    /// </summary>
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

    /// <summary>
    /// Modelo da página de entrada (Home Page) do portal RallyFestas.
    /// Esta classe é encarregue de carregar as contagens gerais para o painel de estatísticas,
    /// a lista de eventos mais próximos no calendário e as coordenadas dos locais para o mapa interativo.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Contexto de acesso à base de dados, injetado via construtor.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do modelo da página inicial.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista dos próximos eventos agendados que ainda não terminaram.
        /// </summary>
        public IList<Evento> EventosProximos { get; set; } = [];

        /// <summary>
        /// Lista dos principais locais registados para exibição rápida.
        /// </summary>
        public IList<Local> LocaisPrincipais { get; set; } = [];

        /// <summary>
        /// Lista de DTOs contendo os dados dos eventos georreferenciados para renderização no mapa interativo.
        /// </summary>
        public IList<MapEventDto> EventosMapa { get; set; } = [];

        /// <summary>
        /// Número total de artistas registados no sistema.
        /// </summary>
        public int TotalArtistas { get; set; }

        /// <summary>
        /// Número total de eventos agendados no sistema.
        /// </summary>
        public int TotalEventos { get; set; }

        /// <summary>
        /// Número total de locais registados no sistema.
        /// </summary>
        public int TotalLocais { get; set; }

        /// <summary>
        /// Processa o pedido HTTP GET para a página inicial.
        /// Carrega assincronamente as estatísticas, próximos eventos e dados do mapa da base de dados.
        /// </summary>
        /// <returns>Uma tarefa assíncrona que representa a operação de carregamento.</returns>
        public async Task OnGetAsync()
        {
            var agora = DateTime.Now;

            // Obtém os próximos 6 eventos agendados cujo horário de fim seja posterior ao momento atual
            EventosProximos = await _context.Eventos
                .Include(e => e.Local)
                .Where(e => e.DataFim >= agora)
                .OrderBy(e => e.DataInicio)
                .Take(6)
                .ToListAsync();

            // Carrega os primeiros 3 locais registados para fins de apresentação resumida na página
            LocaisPrincipais = await _context.Locais
                .Take(3)
                .ToListAsync();

            // Efetua as contagens totais para alimentar as caixas de estatísticas rápidas
            TotalArtistas = await _context.Artistas.CountAsync();
            TotalEventos = await _context.Eventos.CountAsync();
            TotalLocais = await _context.Locais.CountAsync();

            // Obtém todos os eventos que tenham um local atribuído e que possuam coordenadas geográficas registadas
            var eventosComCoordenadas = await _context.Eventos
                .Include(e => e.Local)
                .Where(e => e.Local != null && !string.IsNullOrEmpty(e.Local.Coordenadas))
                .ToListAsync();

            // Filtra coordenadas que sejam URLs (ex: Google Maps) e projeta as coordenadas do tipo latitude,longitude para o DTO do mapa
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
