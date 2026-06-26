using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

/// <summary>
/// Modelo da página de detalhes de um Artista.
/// Carrega a ficha informativa de um artista, incluindo a sua foto, biografia, 
/// e toda a sua agenda de atuações (eventos e respetivos locais).
/// </summary>
[Authorize]
public class DetailsModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de detalhes de artistas.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto contendo os dados detalhados do Artista a apresentar.
    /// </summary>
    public Artista Artista { get; set; } = default!;

    /// <summary>
    /// Parâmetro HTTP GET opcional que define a origem do utilizador (ex: 'admin') para fins de redirecionamento de regresso.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? From { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET.
    /// Efetua o carregamento do artista e a árvore de relacionamentos necessária (Cartazes -> Evento -> Local).
    /// </summary>
    /// <param name="id">O identificador do artista.</param>
    /// <returns>A página de detalhes ou erro 404 (NotFound) se o artista não for encontrado.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        // Carrega o artista com eager loading das atuações, eventos e locais para evitar o problema de N+1 consultas
        var artista = await _context.Artistas
            .Include(a => a.Cartazes)
                .ThenInclude(c => c.Evento)
                    .ThenInclude(e => e.Local)
            .FirstOrDefaultAsync(m => m.IdArtista == id);

        if (artista is null) return NotFound();

        Artista = artista;
        return Page();
    }
}
