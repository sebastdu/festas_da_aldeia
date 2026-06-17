using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private const int ItensPorPagina = 9;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Local> Local { get; set; } = default!;

    [BindProperty(SupportsGet = true)] public int Pagina { get; set; } = 1;
    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Locais
            .Include(l => l.Eventos)
            .OrderBy(l => l.Nome)
            .AsQueryable();

        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        Local = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
