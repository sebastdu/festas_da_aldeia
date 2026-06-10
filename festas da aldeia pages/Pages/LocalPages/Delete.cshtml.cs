using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Local Local { get; set; } = default!;

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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var local = await _context.Locais.Include(l => l.Eventos).FirstOrDefaultAsync(l => l.IdLocal == id);
        if (local != null)
        {
            Local = local;

            // Validação customizada: verificar se existem eventos associados
            if (Local.Eventos != null && Local.Eventos.Any())
            {
                ModelState.AddModelError(string.Empty, "Não é possível eliminar o local porque existem eventos associados a ele.");
                return Page();
            }

            _context.Locais.Remove(Local);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
