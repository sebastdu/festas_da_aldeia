using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

/// <summary>
/// Modelo da página pública individual de um Local (ex: detalhes com mapa e eventos próprios).
/// Carrega as informações detalhadas de um local e a lista completa de eventos agendados para o mesmo.
/// </summary>
[Authorize]
public class PaginaLocaisModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página individual do local.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public PaginaLocaisModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto que contém as informações do Local a apresentar.
    /// </summary>
    public Local Local { get; set; } = default!;

    /// <summary>
    /// Carrega as informações e os eventos de um local com base no ID fornecido via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador do local a carregar.</param>
    /// <returns>A página individual do local ou erro 404 (NotFound) se o local não existir.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        // Carrega o local e os respetivos eventos associados
        var local = await _context.Locais
            .Include(l => l.Eventos)
            .FirstOrDefaultAsync(l => l.IdLocal == id);

        if (local is null) return NotFound();

        Local = local;
        return Page();
    }
}
