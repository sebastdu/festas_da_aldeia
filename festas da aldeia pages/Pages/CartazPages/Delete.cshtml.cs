using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

/// <summary>
/// Modelo da página de eliminação de uma atuação do Cartaz.
/// Permite que Administradores cancelem e removam atuações de artistas de eventos agendados.
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de eliminação de atuações.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto que contém os dados da atuação (Cartaz) a remover.
    /// </summary>
    [BindProperty]
    public Cartaz Cartaz { get; set; } = default!;

    /// <summary>
    /// Carrega a atuação e apresenta a página de confirmação de eliminação quando acedida via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador da atuação.</param>
    /// <returns>A página de confirmação ou erro 404 (NotFound) se a atuação não for encontrada.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var cartaz = await _context.Cartazes.FirstOrDefaultAsync(m => m.IdCartaz == id);
        if (cartaz is null)
        {
            return NotFound();
        }
        else
        {
            Cartaz = cartaz;
        }

        return Page();
    }

    /// <summary>
    /// Processa a eliminação da atuação do cartaz via HTTP POST após confirmação.
    /// Remove o registo correspondente e grava as alterações na base de dados.
    /// </summary>
    /// <param name="id">O identificador da atuação a remover.</param>
    /// <returns>Redireciona para o índice de atuações.</returns>
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var cartaz = await _context.Cartazes.FindAsync(id);
        if (cartaz != null)
        {
            Cartaz = cartaz;

            // Remove o registo de atuação da base de dados
            _context.Cartazes.Remove(Cartaz);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
