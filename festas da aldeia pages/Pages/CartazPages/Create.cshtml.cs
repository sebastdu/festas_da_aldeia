using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public SelectList ArtistasSelectList { get; set; } = default!;
    public SelectList EventosSelectList { get; set; } = default!;

    public IActionResult OnGet()
    {
        ArtistasSelectList = new SelectList(_context.Artistas, "IdArtista", "Nome");
        EventosSelectList = new SelectList(_context.Eventos, "IdEvento", "Nome");
        return Page();
    }

    [BindProperty]
    public Cartaz Cartaz { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        // Ignorar a validação das propriedades de navegação no post
        ModelState.Remove("Cartaz.Artista");
        ModelState.Remove("Cartaz.Evento");

        if (!ModelState.IsValid)
        {
            ArtistasSelectList = new SelectList(_context.Artistas, "IdArtista", "Nome");
            EventosSelectList = new SelectList(_context.Eventos, "IdEvento", "Nome");
            return Page();
        }

        _context.Cartazes.Add(Cartaz);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
