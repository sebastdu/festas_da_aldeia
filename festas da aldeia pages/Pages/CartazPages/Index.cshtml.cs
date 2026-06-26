using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

/// <summary>
/// Modelo da página de consulta pública e administração de Cartazes (atuações).
/// Suporta filtros de pesquisa por artista, evento ou local, e paginação dos resultados obtidos.
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Limite constante de atuações a apresentar por página.
    /// </summary>
    private const int ItensPorPagina = 8;

    /// <summary>
    /// Construtor do modelo da listagem de atuações no cartaz.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Coleção contendo as atuações a apresentar no lote/página selecionada.
    /// </summary>
    public IList<Cartaz> Cartaz { get; set; } = [];

    /// <summary>
    /// Termo de pesquisa introduzido pelo utilizador para filtrar por artista, evento ou local (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public string? Pesquisa { get; set; }

    /// <summary>
    /// O número correspondente à página de paginação ativa (HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Total de páginas geradas após aplicação dos filtros de pesquisa.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Número total absoluto de registos de atuações encontrados.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET assincronamente.
    /// Efetua a filtragem por texto livre nos nomes dos artistas, eventos e locais e pagina o resultado.
    /// </summary>
    /// <returns>Uma tarefa assíncrona que representa a operação de carregamento.</returns>
    public async Task OnGetAsync()
    {
        // Inicia a consulta base carregando os dados do Artista, Evento e o respetivo Local associado
        var query = _context.Cartazes
            .Include(c => c.Artista)
            .Include(c => c.Evento)
                .ThenInclude(e => e.Local)
            .AsQueryable();

        // Aplica o filtro de texto se o utilizador especificou uma pesquisa
        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var termo = Pesquisa.Trim().ToLower();
            query = query.Where(c =>
                c.Artista.Nome.ToLower().Contains(termo) ||
                c.Evento.Nome.ToLower().Contains(termo) ||
                (c.Evento.Local != null && (
                    c.Evento.Local.Nome.ToLower().Contains(termo) ||
                    (c.Evento.Local.Descricao != null && c.Evento.Local.Descricao.ToLower().Contains(termo))
                ))
            );
        }

        // Ordena as atuações cronologicamente pela data e hora de atuação
        query = query.OrderBy(c => c.DataHoraAtuacao);

        // Calcula a paginação de forma segura contra páginas fora do intervalo
        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        // Seleciona apenas os registos correspondentes à página ativa
        Cartaz = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}