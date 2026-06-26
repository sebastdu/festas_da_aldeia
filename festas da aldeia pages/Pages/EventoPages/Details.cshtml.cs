using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

/// <summary>
/// Modelo da página de detalhes administrativos/públicos de um Evento específico.
/// Carrega de forma otimizada os dados geográficos do local e as atuações agendadas no cartaz,
/// incluindo os dados biográficos e perfis dos respetivos artistas.
/// </summary>
public class DetailsModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de detalhes do evento.
    /// </summary>
    /// <param name="context">O contexto de base de dados da aplicação.</param>
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// O Evento carregado com todos os dados associados a apresentar na view.
    /// </summary>
    public Evento Evento { get; set; } = default!;

    /// <summary>
    /// Processa o pedido HTTP GET assincronamente.
    /// Carrega as informações detalhadas do evento e as suas relações com Local, Cartazes e Artistas.
    /// </summary>
    /// <param name="id">O identificador único do evento.</param>
    /// <returns>A página de detalhes ou NotFound caso o identificador seja nulo ou inexistente.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        // Carrega o evento por ID, efetuando Eager Loading do Local e das Atuações (Cartaz),
        // estendendo a pesquisa para incluir os objetos Artista correspondentes.
        var evento = await _context.Eventos
            .Include(e => e.Local)
            .Include(e => e.Cartazes)
                .ThenInclude(c => c.Artista)
            .FirstOrDefaultAsync(m => m.IdEvento == id);

        if (evento is null)
        {
            return NotFound();
        }
        else
        {
            Evento = evento;
        }

        return Page();
    }
}
