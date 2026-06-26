using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

/// <summary>
/// Modelo da página de consulta e filtragem de Locais.
/// Fornece funcionalidades de pesquisa por termo de texto e filtragem dinâmica (espaço exterior vs. interior),
/// gerando uma lista filtrada ordenada alfabeticamente.
/// </summary>
public class ListaLocaisModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de listagem de locais com filtros.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public ListaLocaisModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista dos locais que correspondem aos critérios de filtragem e pesquisa ativos.
    /// </summary>
    public IList<Local> Locais { get; set; } = [];

    /// <summary>
    /// Termo de pesquisa introduzido pelo utilizador (mapeado via parâmetro HTTP GET).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public string? Pesquisa { get; set; }

    /// <summary>
    /// Critério de filtro selecionado pelo utilizador (opções: "exterior", "interior" ou todos).
    /// </summary>
    [BindProperty(SupportsGet = true)] 
    public string? Filtro { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET.
    /// Aplica as regras de pesquisa por nome e filtragem por tipo de espaço, ordenando os resultados.
    /// </summary>
    /// <returns>Uma tarefa assíncrona que representa a operação de carregamento e filtragem.</returns>
    public async Task OnGetAsync()
    {
        var query = _context.Locais
            .Include(l => l.Eventos)
            .AsQueryable();

        // Filtra os locais pelo termo de pesquisa inserido (ignora maiúsculas/minúsculas)
        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var termo = Pesquisa.Trim().ToLower();
            query = query.Where(l => l.Nome.ToLower().Contains(termo));
        }

        // Aplica o filtro de espaço exterior ou interior
        query = Filtro switch
        {
            "exterior" => query.Where(l => l.Outside),
            "interior" => query.Where(l => !l.Outside),
            _ => query
        };

        // Carrega a lista final ordenada alfabeticamente
        Locais = await query.OrderBy(l => l.Nome).ToListAsync();
    }
}
