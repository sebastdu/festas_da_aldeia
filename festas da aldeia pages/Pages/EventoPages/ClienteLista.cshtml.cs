using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

/// <summary>
/// Modelo da página de consulta pública de listagem de Eventos destinada aos clientes/utilizadores finais.
/// Permite efetuar pesquisas por nome do artista, evento ou local, filtrar pelo estado do evento
/// (em curso, brevemente, concluídos) e realizar a paginação de forma flexível.
/// </summary>
public class ClienteListaModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Limite constante de eventos por página para consulta pública.
    /// </summary>
    private const int ItensPorPagina = 8;

    /// <summary>
    /// Construtor do modelo de listagem de eventos para clientes.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public ClienteListaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista dos eventos a apresentar na página ativa.
    /// </summary>
    public IList<Evento> Eventos { get; set; } = [];

    /// <summary>
    /// Coleção completa de locais para alimentar o filtro por local (dropdown) na view.
    /// </summary>
    public IList<Local> Locais { get; set; } = [];

    /// <summary>
    /// Indica se a base de dados não contém quaisquer registos de eventos.
    /// </summary>
    public bool BancoVazio { get; set; }

    /// <summary>
    /// O número correspondente à página de paginação ativa (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Termo de pesquisa por texto livre (nome, descrição ou artista) (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public string? Pesquisa { get; set; }

    /// <summary>
    /// Identificador do local para filtrar eventos desse local (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public int? LocalId { get; set; }

    /// <summary>
    /// Filtro de estado temporal ("decorrer", "brevemente", "passados") (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public string? Estado { get; set; }

    /// <summary>
    /// Filtro de data específica para encontrar eventos ativos nesse dia (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public DateTime? Data { get; set; }

    /// <summary>
    /// Total de páginas calculadas com base nas restrições de filtragem ativas.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Total absoluto de registos de eventos resultantes da filtragem.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET assincronamente.
    /// Aplica os múltiplos filtros parametrizados e ordena os resultados dando prioridade a eventos "em curso".
    /// </summary>
    /// <returns>Uma tarefa assíncrona que representa a operação.</returns>
    public async Task OnGetAsync()
    {
        // Verifica se a base de dados de eventos se encontra vazia
        BancoVazio = !await _context.Eventos.AnyAsync();
        
        // Obtém a lista ordenada de locais para a dropdown
        Locais = await _context.Locais.OrderBy(l => l.Nome).ToListAsync();

        // Constrói a query base com carregamento das relações de Local e Artistas
        var query = _context.Eventos
            .Include(e => e.Local)
            .Include(e => e.Cartazes)
                .ThenInclude(c => c.Artista)
            .AsQueryable();

        // 1. Filtro: Pesquisa textual por Nome do evento, Descrição ou Nome do Artista escalado no cartaz
        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var p = Pesquisa.Trim().ToLower();
            query = query.Where(e => e.Nome.ToLower().Contains(p) 
                                     || (e.Descricao != null && e.Descricao.ToLower().Contains(p))
                                     || e.Cartazes.Any(c => c.Artista != null && c.Artista.Nome.ToLower().Contains(p)));
        }

        // 2. Filtro: Filtragem por local selecionado
        if (LocalId.HasValue)
        {
            query = query.Where(e => e.IdLocal == LocalId.Value);
        }

        // 3. Filtro: Estado temporal do evento (decorrer, brevemente ou passados)
        if (!string.IsNullOrEmpty(Estado))
        {
            var agora = DateTime.Now;
            if (Estado == "decorrer")
            {
                query = query.Where(e => e.DataInicio <= agora && e.DataFim >= agora);
            }
            else if (Estado == "brevemente")
            {
                query = query.Where(e => e.DataInicio > agora);
            }
            else if (Estado == "passados")
            {
                query = query.Where(e => e.DataFim < agora);
            }
        }

        // 4. Filtro: Eventos ativos numa data calendarizada específica
        if (Data.HasValue)
        {
            var d = Data.Value.Date;
            query = query.Where(e => e.DataInicio.Date <= d && e.DataFim.Date >= d);
        }

        // Ordenação inteligente: 
        // 0 -> Eventos em curso (decorrer)
        // 1 -> Eventos futuros (brevemente)
        // 2 -> Eventos passados
        // Dentro de cada grupo, ordena cronologicamente por DataInicio
        var agoraParaOrdem = DateTime.Now;
        query = query
            .OrderBy(e => (e.DataInicio <= agoraParaOrdem && e.DataFim >= agoraParaOrdem) ? 0 :
                          (e.DataInicio > agoraParaOrdem) ? 1 : 2)
            .ThenBy(e => e.DataInicio);

        // Calcula os limites de paginação de forma segura
        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        // Carrega o lote da página selecionada
        Eventos = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
