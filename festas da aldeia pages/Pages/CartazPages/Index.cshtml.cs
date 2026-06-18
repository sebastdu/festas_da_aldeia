using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private const int ItensPorPagina = 8;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Cartaz> Cartaz { get; set; } = [];

    [BindProperty(SupportsGet = true)] public string? Pesquisa { get; set; }
    [BindProperty(SupportsGet = true)] public int Pagina { get; set; } = 1;

    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Cartazes
            .Include(c => c.Artista)
            .Include(c => c.Evento)
                .ThenInclude(e => e.Local)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var termo = Pesquisa.Trim().ToLower();
            query = query.Where(c =>
                c.Artista.Nome.ToLower().Contains(termo) ||
                c.Evento.Nome.ToLower().Contains(termo) ||
                (c.Evento.Local != null && (
                    c.Evento.Local.Nome.ToLower().Contains(termo) ||
                    (c.Evento.Local.Descricao != null && c.Evento.Local.Descricao.ToLower().Contains(termo))
                ))
            );
        }

        query = query.OrderBy(c => c.DataHoraAtuacao);

        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        Cartaz = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}