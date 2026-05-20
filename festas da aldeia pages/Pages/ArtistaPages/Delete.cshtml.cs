using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Artista Artista { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var artista = await _context.Artistas.FirstOrDefaultAsync(m => m.IdArtista == id);
        if (artista is null)
        {
            return NotFound();
        }
        else
        {
            Artista = artista;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var artista = await _context.Artistas.FindAsync(id);
        if (artista != null)
        {
            Artista = artista;
            _context.Artistas.Remove(Artista);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
