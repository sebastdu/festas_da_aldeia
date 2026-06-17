using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private const int ItensPorPagina = 8;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Artista> Artista { get; set; } = default!;

    [BindProperty(SupportsGet = true)] public string? Ordem { get; set; }
    [BindProperty(SupportsGet = true)] public string? Pesquisa { get; set; }
    [BindProperty(SupportsGet = true)] public int Pagina { get; set; } = 1;

    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Artistas.Include(a => a.Cartazes).AsQueryable();

        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var termo = Pesquisa.Trim().ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(termo));
        }

        query = Ordem switch
        {
            "za" => query.OrderByDescending(a => a.Nome),
            "mais" => query.OrderByDescending(a => a.Cartazes.Count),
            "menos" => query.OrderBy(a => a.Cartazes.Count),
            _ => query.OrderBy(a => a.Nome)
        };

        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        Artista = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
