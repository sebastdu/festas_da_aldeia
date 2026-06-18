using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

public class ClienteListaModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private const int ItensPorPagina = 8;

    public ClienteListaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Evento> Eventos { get; set; } = [];

    [BindProperty(SupportsGet = true)] public int Pagina { get; set; } = 1;

    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Eventos
            .Include(e => e.Local)
            .Include(e => e.Cartazes)
            .OrderBy(e => e.DataInicio)
            .AsQueryable();

        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        Eventos = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
