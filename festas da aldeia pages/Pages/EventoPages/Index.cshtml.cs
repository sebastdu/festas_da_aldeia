using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

/// <summary>
/// Modelo da página de listagem administrativa de Eventos.
/// Carrega e apresenta a lista de eventos com suporte para paginação.
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Limite constante de eventos exibidos por página.
    /// </summary>
    private const int ItensPorPagina = 9;

    /// <summary>
    /// Construtor do modelo da listagem administrativa de eventos.
    /// </summary>
    /// <param name="context">O contexto de base de dados da aplicação.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// A lista de eventos a apresentar na página ativa.
    /// </summary>
    public IList<Evento> Evento { get; set; } = default!;

    /// <summary>
    /// O número correspondente à página de paginação ativa (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Total de páginas calculadas com base no volume de registos existentes.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Contagem total absoluta de eventos registados na base de dados.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET assincronamente.
    /// Carrega os eventos ordenados cronologicamente pelo seu início e aplica a paginação.
    /// </summary>
    /// <returns>Uma tarefa assíncrona que representa a operação de carregamento.</returns>
    public async Task OnGetAsync()
    {
        // Constrói a consulta incluindo a entidade Local associada
        var query = _context.Eventos
            .Include(e => e.Local)
            .OrderBy(e => e.DataInicio)
            .AsQueryable();

        // Calcula os totais de páginas e itens
        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        // Obtém o lote correspondente aos limites da página selecionada
        Evento = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
