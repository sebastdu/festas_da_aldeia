using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
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
        Evento = evento;

        ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome", Evento.IdLocal);
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove as propriedades de navegação da validação, 
        // pois elas não são preenchidas no formulário de edição.
        ModelState.Remove("Evento.Local");
        ModelState.Remove("Evento.Cartazes");

        // Validação customizada: verificar se o local selecionado existe
        var localExists = await _context.Locais.AnyAsync(l => l.IdLocal == Evento.IdLocal);
        if (!localExists)
        {
            ModelState.AddModelError("Evento.IdLocal", "O local selecionado é inválido.");
        }

        // Validação customizada: data de fim posterior à data de início
        if (Evento.DataFim <= Evento.DataInicio)
        {
            ModelState.AddModelError("Evento.DataFim", "A data de fim deve ser posterior à data de início.");
        }

        // Validação customizada: sobreposição de eventos no mesmo local (excluindo este evento)
        if (localExists && Evento.DataFim > Evento.DataInicio)
        {
            bool hasOverlap = await _context.Eventos.AnyAsync(e =>
                e.IdLocal == Evento.IdLocal &&
                e.IdEvento != Evento.IdEvento &&
                Evento.DataInicio < e.DataFim &&
                Evento.DataFim > e.DataInicio);

            if (hasOverlap)
            {
                ModelState.AddModelError("Evento.IdLocal", "Já existe outro evento agendado para este local neste intervalo de tempo.");
            }
        }

        if (!ModelState.IsValid)
        {
            ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome", Evento.IdLocal);
            return Page();
        }

        Evento.Nome = Evento.Nome.Trim();
        if (Evento.Descricao != null)
        {
            Evento.Descricao = Evento.Descricao.Trim();
        }
        if (Evento.Patrocinador != null)
        {
            Evento.Patrocinador = Evento.Patrocinador.Trim();
        }

        _context.Attach(Evento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventoExists(Evento.IdEvento))
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

    private bool EventoExists(int id)
    {
        return _context.Eventos.Any(e => e.IdEvento == id);
    }
}
