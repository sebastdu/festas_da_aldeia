using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

/// <summary>
/// Modelo da página de eliminação de Eventos.
/// Restrito a utilizadores com privilégios de "Admin".
/// Garante que eventos com atuações de artistas associadas no cartaz não sejam eliminados acidentalmente.
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de eliminação.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// O Evento a ser eliminado, mapeado para a view e para o pedido HTTP POST.
    /// </summary>
    [BindProperty]
    public Evento Evento { get; set; } = default!;

    /// <summary>
    /// Processa o pedido HTTP GET assincronamente.
    /// Carrega o evento especificado pelo identificador para apresentação no formulário de confirmação.
    /// </summary>
    /// <param name="id">O identificador único do evento.</param>
    /// <returns>A página de confirmação de eliminação, ou NotFound se o ID for nulo ou inválido.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var evento = await _context.Eventos.FirstOrDefaultAsync(m => m.IdEvento == id);
        if (evento is null)
        {
            return NotFound();
        }
        else
        {
            Evento = evento;
        }

        return Page();
    }

    /// <summary>
    /// Processa o pedido HTTP POST para confirmar a eliminação de um evento.
    /// Verifica se o evento possui atuações agendadas no cartaz e, caso possua, impede a eliminação.
    /// </summary>
    /// <param name="id">O identificador único do evento.</param>
    /// <returns>
    /// Redireciona para a listagem em caso de sucesso;
    /// recarrega a página atual com mensagens de erro caso existam restrições de integridade.
    /// </returns>
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        // Carrega o evento incluindo a lista de atuações (Cartaz) associadas para validar restrições
        var evento = await _context.Eventos.Include(e => e.Cartazes).FirstOrDefaultAsync(e => e.IdEvento == id);
        if (evento != null)
        {
            Evento = evento;

            // Validação de negócio: impedir a eliminação se existirem artistas escalados no cartaz
            if (Evento.Cartazes != null && Evento.Cartazes.Any())
            {
                ModelState.AddModelError(string.Empty, "Não é possível eliminar o evento porque existem artistas associados a ele no cartaz.");
                return Page();
            }

            // Remove o registo e persiste a alteração na base de dados
            _context.Eventos.Remove(Evento);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
