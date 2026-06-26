using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

/// <summary>
/// Modelo da página de eliminação de um Local.
/// Permite que Administradores removam recintos, garantindo previamente que não existem 
/// eventos associados a esses locais para manter a integridade referencial dos dados.
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de eliminação de locais.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto contendo os dados do Local a ser eliminado, vinculado à página de confirmação.
    /// </summary>
    [BindProperty]
    public Local Local { get; set; } = default!;

    /// <summary>
    /// Carrega os dados do local especificado pelo ID para a página de confirmação quando acedida via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador do local a eliminar.</param>
    /// <returns>A página de confirmação de eliminação ou erro 404 (NotFound) se o local não existir.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var local = await _context.Locais.FirstOrDefaultAsync(m => m.IdLocal == id);
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

    /// <summary>
    /// Processa a eliminação efetiva do local via HTTP POST após confirmação do utilizador.
    /// Valida se existem eventos dependentes deste local; se existirem, impede a remoção com um erro de validação.
    /// </summary>
    /// <param name="id">O identificador do local a remover.</param>
    /// <returns>
    /// Redireciona para o índice de locais se for removido com sucesso;
    /// caso contrário, recarrega a página de confirmação exibindo o erro de bloqueio.
    /// </returns>
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        // Obtém o local e inclui os eventos associados para validação de integridade referencial
        var local = await _context.Locais.Include(l => l.Eventos).FirstOrDefaultAsync(l => l.IdLocal == id);
        if (local != null)
        {
            Local = local;

            // Validação customizada: impede a eliminação se houver eventos associados para evitar registos órfãos
            if (Local.Eventos != null && Local.Eventos.Any())
            {
                ModelState.AddModelError(string.Empty, "Não é possível eliminar o local porque existem eventos associados a ele.");
                return Page();
            }

            // Remove o local do contexto e grava as alterações no banco de dados
            _context.Locais.Remove(Local);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
