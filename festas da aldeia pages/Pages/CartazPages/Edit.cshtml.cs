using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Cartaz Cartaz { get; set; } = default!;

    public SelectList ArtistasSelectList { get; set; } = default!;
    public SelectList EventosSelectList { get; set; } = default!;

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
        Cartaz = cartaz;
        ArtistasSelectList = new SelectList(_context.Artistas, "IdArtista", "Nome");
        EventosSelectList = new SelectList(_context.Eventos, "IdEvento", "Nome");
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("Cartaz.Artista");
        ModelState.Remove("Cartaz.Evento");

        if (!ModelState.IsValid)
        {
            ArtistasSelectList = new SelectList(_context.Artistas, "IdArtista", "Nome");
            EventosSelectList = new SelectList(_context.Eventos, "IdEvento", "Nome");
            return Page();
        }

        _context.Attach(Cartaz).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CartazExists(Cartaz.IdCartaz))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool CartazExists(int id)
    {
        return _context.Cartazes.Any(e => e.IdCartaz == id);
    }
}
