using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

/// <summary>
/// Modelo da página de detalhes de uma atuação do Cartaz.
/// Carrega os dados detalhados da atuação, bem como o Artista e o Evento associados, 
/// incluindo o Local físico da atuação.
/// </summary>
[Authorize]
public class DetailsModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de detalhes de cartazes.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto que contém as informações da atuação (Cartaz) a apresentar.
    /// </summary>
    public Cartaz Cartaz { get; set; } = default!;

    /// <summary>
    /// Carrega as informações detalhadas da atuação especificada por ID quando a página é acedida via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador da atuação.</param>
    /// <returns>A página de detalhes ou erro 404 (NotFound) se a atuação não existir.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        // Carrega o cartaz e faz eager loading dos dados relacionados (Artista, Evento e o respetivo Local)
        var cartaz = await _context.Cartazes
            .Include(c => c.Artista)
            .Include(c => c.Evento)
                .ThenInclude(e => e.Local)
            .FirstOrDefaultAsync(m => m.IdCartaz == id);

        if (cartaz is null) return NotFound();

        Cartaz = cartaz;
        return Page();
    }
}
