using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateViewDataAsync();
        return Page();
    }

    [BindProperty]
    public Cartaz Cartaz { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("Cartaz.Artista");
        ModelState.Remove("Cartaz.Evento");

        if (!ModelState.IsValid)
        {
            await PopulateViewDataAsync();
            return Page();
        }

        // Validar se a atuação está dentro do período do evento
        var evento = await _context.Eventos.FindAsync(Cartaz.IdEvento);
        if (evento != null)
        {
            var fimAtuacao = Cartaz.DataHoraAtuacao.AddMinutes(Cartaz.DuracaoMinutos);

            if (Cartaz.DataHoraAtuacao < evento.DataInicio)
            {
                ModelState.AddModelError("Cartaz.DataHoraAtuacao",
                    $"A atuação não pode começar antes do início do evento ({evento.DataInicio:dd/MM/yyyy HH:mm}).");
            }
            else if (fimAtuacao > evento.DataFim)
            {
                ModelState.AddModelError("Cartaz.DataHoraAtuacao",
                    $"A atuação termina às {fimAtuacao:HH:mm} mas o evento acaba às {evento.DataFim:dd/MM/yyyy HH:mm}.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewDataAsync();
                return Page();
            }
        }

        _context.Cartazes.Add(Cartaz);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task PopulateViewDataAsync()
    {
        var artistas = await _context.Artistas.OrderBy(a => a.Nome).ToListAsync();
        var eventos = await _context.Eventos.OrderBy(e => e.Nome).ToListAsync();

        ViewData["IdArtista"] = new SelectList(artistas, nameof(Artista.IdArtista), nameof(Artista.Nome));
        ViewData["IdEvento"] = new SelectList(eventos, nameof(Evento.IdEvento), nameof(Evento.Nome));
    }
}
