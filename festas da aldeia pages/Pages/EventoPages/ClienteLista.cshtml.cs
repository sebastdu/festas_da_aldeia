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
    public IList<Local> Locais { get; set; } = [];
    public bool BancoVazio { get; set; }

    [BindProperty(SupportsGet = true)] public int Pagina { get; set; } = 1;
    [BindProperty(SupportsGet = true)] public string? Pesquisa { get; set; }
    [BindProperty(SupportsGet = true)] public int? LocalId { get; set; }
    [BindProperty(SupportsGet = true)] public string? Estado { get; set; }

    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }

    public async Task OnGetAsync()
    {
        BancoVazio = !await _context.Eventos.AnyAsync();
        Locais = await _context.Locais.OrderBy(l => l.Nome).ToListAsync();

        var query = _context.Eventos
            .Include(e => e.Local)
            .Include(e => e.Cartazes)
                .ThenInclude(c => c.Artista)
            .AsQueryable();

        // 1. Pesquisa por Nome, Descrição ou Artista
        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            var p = Pesquisa.Trim().ToLower();
            query = query.Where(e => e.Nome.ToLower().Contains(p) 
                                     || (e.Descricao != null && e.Descricao.ToLower().Contains(p))
                                     || e.Cartazes.Any(c => c.Artista != null && c.Artista.Nome.ToLower().Contains(p)));
        }

        // 2. Filtro de Local
        if (LocalId.HasValue)
        {
            query = query.Where(e => e.IdLocal == LocalId.Value);
        }

        // 3. Filtro de Estado (decorrer, brevemente, passados)
        if (!string.IsNullOrEmpty(Estado))
        {
            var agora = DateTime.Now;
            if (Estado == "decorrer")
            {
                query = query.Where(e => e.DataInicio <= agora && e.DataFim >= agora);
            }
            else if (Estado == "brevemente")
            {
                query = query.Where(e => e.DataInicio > agora);
            }
            else if (Estado == "passados")
            {
                query = query.Where(e => e.DataFim < agora);
            }
        }

        query = query.OrderBy(e => e.DataInicio);

        TotalItens = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)ItensPorPagina);
        Pagina = Math.Max(1, Math.Min(Pagina, TotalPaginas == 0 ? 1 : TotalPaginas));

        Eventos = await query
            .Skip((Pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
