using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

public class PaginaLocaisModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public PaginaLocaisModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Local Local { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var local = await _context.Locais
            .Include(l => l.Eventos)
            .FirstOrDefaultAsync(l => l.IdLocal == id);

        if (local is null) return NotFound();

        Local = local;
        return Page();
    }
}
