using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

/// <summary>
/// Modelo da página de detalhes de um Local.
/// Permite carregar e expor as informações completas de um local específico (como mapa e recintos) 
/// e listar todos os eventos atualmente agendados para este espaço.
/// </summary>
public class DetailsModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de detalhes de locais.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto que contém as informações do Local a apresentar.
    /// </summary>
    public Local Local { get; set; } = default!;

    /// <summary>
    /// Carrega as informações do local e a sua lista de eventos associados quando o pedido é efetuado via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador único do local.</param>
    /// <returns>A página de detalhes ou erro 404 (NotFound) se o local não for encontrado.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        // Obtém o local e faz eager loading da lista de eventos que nele decorrem
        var local = await _context.Locais.Include(l => l.Eventos).FirstOrDefaultAsync(m => m.IdLocal == id);
        if (local is null)
        {
            return NotFound();
        }
        else
        {
            Local = local;
        }

        return Page();
    }
}
