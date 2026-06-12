using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Evento Evento { get; set; } = default!;

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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var evento = await _context.Eventos.Include(e => e.Cartazes).FirstOrDefaultAsync(e => e.IdEvento == id);
        if (evento != null)
        {
            Evento = evento;

            // Validação customizada: impedir a eliminação se existirem artistas escalados no cartaz
            if (Evento.Cartazes != null && Evento.Cartazes.Any())
            {
                ModelState.AddModelError(string.Empty, "Não é possível eliminar o evento porque existem artistas associados a ele no cartaz.");
                return Page();
            }

            _context.Eventos.Remove(Evento);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
