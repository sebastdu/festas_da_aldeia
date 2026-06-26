using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

/// <summary>
/// Modelo da página de consulta pública de Artistas.
/// Suporta filtros de pesquisa por texto livre, ordenação parametrizável (alfabética ou por número de atuações)
/// e paginação dos resultados obtidos.
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Limite constante de artistas por página na grelha pública de visualização.
    /// </summary>
    private const int ItensPorPagina = 8;

    /// <summary>
    /// Construtor do modelo da página inicial pública de artistas.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista dos artistas a apresentar na grelha da página ativa.
    /// </summary>
    public IList<Artista> Artista { get; set; } = default!;

    /// <summary>
    /// Critério de ordenação aplicado à consulta (az, za, mais atuações, menos atuações). Mapeado do HTTP GET.
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public string? Ordem { get; set; }

    /// <summary>
    /// Termo de pesquisa por nome do artista (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public string? Pesquisa { get; set; }

    /// <summary>
    /// O número correspondente à página de paginação ativa (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Total de páginas calculadas com base nos filtros e limite de itens.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Total absoluto de artistas correspondentes à consulta filtrada.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET.
    /// Aplica as regras de filtragem por texto livre, ordenação condicional e paginação assíncrona.
    /// </summary>
    /// <returns>Uma tarefa assíncrona que representa a operação de carregamento.</returns>
    public async Task OnGetAsync()
    {
        // Constrói a consulta base com eager loading das atuações para otimizar a contagem
        var query = _context.Artistas.Include(a => a.Cartazes).AsQueryable();

        // Filtra pelo nome do artista (se o utilizador introduziu termo de pesquisa)
        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var termo = Pesquisa.Trim().ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(termo));
        }

        // Aplica a ordenação segundo a escolha do utilizador
        query = Ordem switch
        {
            "za" => query.OrderByDescending(a => a.Nome),
            "mais" => query.OrderByDescending(a => a.Cartazes.Count),
            "menos" => query.OrderBy(a => a.Cartazes.Count),
            _ => query.OrderBy(a => a.Nome)
        };

        // Calcula os limites de paginação de forma defensiva
        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        // Obtém o lote de artistas correspondente
        Artista = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
