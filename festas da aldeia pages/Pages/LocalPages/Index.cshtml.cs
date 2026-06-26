using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

/// <summary>
/// Modelo da página de listagem pública de locais.
/// Carrega e apresenta de forma paginada todos os locais (recintos) registados,
/// incluindo a contagem de eventos associados a cada um.
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Limite constante de itens a apresentar em cada página.
    /// </summary>
    private const int ItensPorPagina = 9;

    /// <summary>
    /// Construtor do modelo da página de listagem de locais.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista dos locais carregados para a página atual.
    /// </summary>
    public IList<Local> Local { get; set; } = default!;

    /// <summary>
    /// O número da página atual a apresentar (suporta parâmetros GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Total acumulado de páginas para efeitos de paginação.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Total de registos de locais encontrados na base de dados.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET.
    /// Calcula a paginação e carrega a lista de locais correspondente à página selecionada.
    /// </summary>
    /// <returns>Uma tarefa assíncrona que representa a operação de carregamento.</returns>
    public async Task OnGetAsync()
    {
        // Constrói a consulta base ordenada alfabeticamente pelo nome do local
        var query = _context.Locais
            .Include(l => l.Eventos)
            .OrderBy(l => l.Nome)
            .AsQueryable();

        // Contabiliza o total de registos para calcular as páginas
        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        
        // Garante que o índice da página está dentro dos limites válidos
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        // Aplica paginação utilizando Skip e Take na base de dados
        Local = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
