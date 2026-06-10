using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Local Local { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var local = await _context.Locais.FirstOrDefaultAsync(m => m.IdLocal == id);
        if (local is null)
        {
            return NotFound();
        }
        Local = local;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("Local.Eventos");

        // Validação customizada: Nome único (excluindo o próprio local)
        if (!string.IsNullOrWhiteSpace(Local.Nome) &&
            await _context.Locais.AnyAsync(l => l.Nome.ToLower() == Local.Nome.Trim().ToLower() && l.IdLocal != Local.IdLocal))
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

        _context.Attach(Local).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LocalExists(Local.IdLocal))
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

    private bool LocalExists(int id)
    {
        return _context.Locais.Any(e => e.IdLocal == id);
    }
}
