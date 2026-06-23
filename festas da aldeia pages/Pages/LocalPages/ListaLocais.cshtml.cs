using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

public class ListaLocaisModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ListaLocaisModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Local> Locais { get; set; } = [];

    [BindProperty(SupportsGet = true)] public string? Pesquisa { get; set; }
    [BindProperty(SupportsGet = true)] public string? Filtro { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Locais
            .Include(l => l.Eventos)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var termo = Pesquisa.Trim().ToLower();
            query = query.Where(l => l.Nome.ToLower().Contains(termo));
        }

        query = Filtro switch
        {
            "exterior" => query.Where(l => l.Outside),
            "interior" => query.Where(l => !l.Outside),
            _ => query
        };

        Locais = await query.OrderBy(l => l.Nome).ToListAsync();
    }
}
