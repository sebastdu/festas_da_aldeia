using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

/// <summary>
/// Modelo da página de visualização de Eventos destinada aos utilizadores finais (público em geral).
/// Carrega o evento e gera dinamicamente uma listagem dos dias de duração do mesmo para filtragem na view.
/// </summary>
[Authorize]
public class ClienteModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página pública de detalhes do evento.
    /// </summary>
    /// <param name="context">O contexto de base de dados da aplicação.</param>
    public ClienteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// O Evento selecionado com os dados e atuações associadas a apresentar na view pública.
    /// </summary>
    public Evento Evento { get; set; } = default!;

    /// <summary>
    /// Lista contendo cada um dos dias únicos em que o evento decorre (para alimentar os filtros por data).
    /// </summary>
    public List<DateTime> DiasEvento { get; set; } = [];

    /// <summary>
    /// Processa o pedido HTTP GET assincronamente.
    /// Carrega o evento, atuações e artistas e calcula o intervalo de datas do evento.
    /// </summary>
    /// <param name="id">O identificador único do evento.</param>
    /// <returns>A página de exibição ao cliente ou NotFound caso o evento não exista ou ID seja inválido.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        // Carrega o evento juntamente com o local e a escala de atuações (com os respetivos artistas)
        var evento = await _context.Eventos
            .Include(e => e.Local)
            .Include(e => e.Cartazes)
                .ThenInclude(c => c.Artista)
            .FirstOrDefaultAsync(m => m.IdEvento == id);

        if (evento is null) return NotFound();

        Evento = evento;

        // Gera sequencialmente a lista de dias compreendidos entre a DataInicio e a DataFim do evento
        var dia = evento.DataInicio.Date;
        while (dia <= evento.DataFim.Date)
        {
            DiasEvento.Add(dia);
            dia = dia.AddDays(1);
        }

        return Page();
    }
}
