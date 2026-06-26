using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

/// <summary>
/// Modelo da página de administração/gestão de Artistas.
/// Permite que utilizadores com o perfil de Administrador consultem e façam a paginação de todos os artistas registados
/// no sistema, incluindo informação sobre as suas atuações associadas.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Número máximo de registos de artistas a exibir em cada página da listagem.
    /// </summary>
    private const int ItensPorPagina = 15;

    /// <summary>
    /// Construtor do modelo da página de administração de artistas.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public AdminModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista dos artistas carregados na página ativa.
    /// </summary>
    public IList<Artista> Artistas { get; set; } = [];

    /// <summary>
    /// O índice numérico da página a apresentar (suporta parâmetros HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Número acumulado de páginas para a paginação.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Total de registos de artistas existentes na base de dados.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET.
    /// Calcula a paginação e carrega a lista de artistas ordenada alfabeticamente.
    /// </summary>
    /// <returns>Uma tarefa assíncrona que representa a operação de carregamento.</returns>
    public async Task OnGetAsync()
    {
        // Cria a consulta de artistas e faz o eager loading das suas atuações (cartazes) e respetivos eventos
        var query = _context.Artistas
            .Include(a => a.Cartazes).ThenInclude(c => c.Evento)
            .OrderBy(a => a.Nome)
            .AsQueryable();

        // Contabiliza o total de registos
        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        
        // Assegura que a página pedida está dentro dos intervalos de índices possíveis
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        // Obtém o segmento correspondente de artistas
        Artistas = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
