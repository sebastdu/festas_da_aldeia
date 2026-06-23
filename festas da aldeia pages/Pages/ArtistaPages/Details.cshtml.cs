using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Artista Artista { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? From { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

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
