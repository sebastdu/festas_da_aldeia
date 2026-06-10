using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;

namespace festas_da_aldeia_pages.Pages.LocalPages;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Local Local { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        // Remove os Eventos da validação, visto que um local novo 
        // inicia sempre com uma lista vazia no formulário de criação.
        ModelState.Remove("Local.Eventos");

        // Validação customizada: Nome único
        if (!string.IsNullOrWhiteSpace(Local.Nome) &&
            await _context.Locais.AnyAsync(l => l.Nome.ToLower() == Local.Nome.Trim().ToLower()))
        {
            ModelState.AddModelError("Local.Nome", "Já existe um local registado com este nome.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Local.Nome = Local.Nome.Trim();
        if (Local.Descricao != null)
        {
            Local.Descricao = Local.Descricao.Trim();
        }
        if (Local.Coordenadas != null)
        {
            Local.Coordenadas = Local.Coordenadas.Trim();
        }

        _context.Locais.Add(Local);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
