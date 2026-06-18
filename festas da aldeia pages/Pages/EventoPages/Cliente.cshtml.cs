using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

public class ClienteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ClienteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Evento Evento { get; set; } = default!;

    // Lista de dias únicos do evento (para os dropdowns)
    public List<DateTime> DiasEvento { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var evento = await _context.Eventos
            .Include(e => e.Local)
            .Include(e => e.Cartazes)
                .ThenInclude(c => c.Artista)
            .FirstOrDefaultAsync(m => m.IdEvento == id);

        if (evento is null) return NotFound();

        Evento = evento;

        // Gerar lista de dias entre DataInicio e DataFim
        var dia = evento.DataInicio.Date;
        while (dia <= evento.DataFim.Date)
        {
            DiasEvento.Add(dia);
            dia = dia.AddDays(1);
        }

        return Page();
    }
}
