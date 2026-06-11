using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;

namespace festas_da_aldeia_pages.Pages.EventoPages;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        // Cria a lista de opções para a dropdown. 
        // Vai guardar o "IdLocal" na BD, mas mostrar o "Nome" ao utilizador.
        ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome");
        return Page();
    }

    [BindProperty]
    public Evento Evento { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        // Remove as propriedades de navegação da validação, 
        // pois elas não são preenchidas no formulário de criação.
        ModelState.Remove("Evento.Local");
        ModelState.Remove("Evento.Cartazes");

        // Validação customizada: verificar se o local selecionado existe
        var localExists = await _context.Locais.AnyAsync(l => l.IdLocal == Evento.IdLocal);
        if (!localExists)
        {
            ModelState.AddModelError("Evento.IdLocal", "O local selecionado é inválido.");
        }

        // Validação customizada: data de início no futuro
        if (Evento.DataInicio < DateTime.Now)
        {
            ModelState.AddModelError("Evento.DataInicio", "A data de início deve ser no futuro.");
        }

        // Validação customizada: data de fim posterior à data de início
        if (Evento.DataFim <= Evento.DataInicio)
        {
            ModelState.AddModelError("Evento.DataFim", "A data de fim deve ser posterior à data de início.");
        }

        // Validação customizada: sobreposição de eventos no mesmo local
        if (localExists && Evento.DataFim > Evento.DataInicio)
        {
            bool hasOverlap = await _context.Eventos.AnyAsync(e =>
                e.IdLocal == Evento.IdLocal &&
                Evento.DataInicio < e.DataFim &&
                Evento.DataFim > e.DataInicio);

            if (hasOverlap)
            {
                ModelState.AddModelError("Evento.IdLocal", "Já existe outro evento agendado para este local neste intervalo de tempo.");
            }
        }

        if (!ModelState.IsValid)
        {
            // Se houver algum erro (ex: faltou o nome), precisamos de recarregar a dropdown
            ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome");
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

        _context.Eventos.Add(Evento);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
