using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

/// <summary>
/// Modelo da página de criação de uma atuação no Cartaz.
/// Permite que Administradores agendem a atuação de um Artista num Evento específico,
/// verificando se o horário e a duração da atuação se inserem dentro dos limites horários do evento.
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de criação de cartazes.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Apresenta o formulário de agendamento de atuações (GET).
    /// Carrega as listas de seleção de artistas e eventos para as dropdowns.
    /// </summary>
    /// <returns>A página que renderiza o formulário.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateViewDataAsync();
        return Page();
    }

    /// <summary>
    /// Objeto que contém os dados da atuação (Cartaz) a ser criada.
    /// </summary>
    [BindProperty]
    public Cartaz Cartaz { get; set; } = default!;

    /// <summary>
    /// Processa a submissão do formulário via HTTP POST.
    /// Valida as datas e duração da atuação em relação ao evento selecionado,
    /// grava o registo e redireciona o utilizador.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice de atuações em caso de sucesso;
    /// caso contrário, recarrega a página atual apresentando os erros de validação correspondentes.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove as entidades de navegação associadas para passar a validação básica do ModelState,
        // pois apenas são submetidas as chaves estrangeiras.
        ModelState.Remove("Cartaz.Artista");
        ModelState.Remove("Cartaz.Evento");

        if (!ModelState.IsValid)
        {
            await PopulateViewDataAsync();
            return Page();
        }

        // Valida defensivamente se a atuação planeada se enquadra dentro do período oficial do evento
        var evento = await _context.Eventos.FindAsync(Cartaz.IdEvento);
        if (evento != null)
        {
            var fimAtuacao = Cartaz.DataHoraAtuacao.AddMinutes(Cartaz.DuracaoMinutos);

            // Verifica se a atuação começa antes do evento se iniciar
            if (Cartaz.DataHoraAtuacao < evento.DataInicio)
            {
                ModelState.AddModelError("Cartaz.DataHoraAtuacao",
                    $"A atuação não pode começar antes do início do evento ({evento.DataInicio:dd/MM/yyyy HH:mm}).");
            }
            // Verifica se a atuação ultrapassa a hora de fecho do evento
            else if (fimAtuacao > evento.DataFim)
            {
                ModelState.AddModelError("Cartaz.DataHoraAtuacao",
                    $"A atuação termina às {fimAtuacao:HH:mm} mas o evento acaba às {evento.DataFim:dd/MM/yyyy HH:mm}.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewDataAsync();
                return Page();
            }
        }

        // Insere a atuação na base de dados
        _context.Cartazes.Add(Cartaz);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    /// <summary>
    /// Carrega as coleções de artistas e eventos e preenche as estruturas ViewData para as dropdowns do formulário.
    /// </summary>
    /// <returns>Uma tarefa assíncrona.</returns>
    private async Task PopulateViewDataAsync()
    {
        var artistas = await _context.Artistas.OrderBy(a => a.Nome).ToListAsync();
        var eventos = await _context.Eventos.OrderBy(e => e.Nome).ToListAsync();

        ViewData["IdArtista"] = new SelectList(artistas, nameof(Artista.IdArtista), nameof(Artista.Nome));
        ViewData["IdEvento"] = new SelectList(eventos, nameof(Evento.IdEvento), nameof(Evento.Nome));
    }
}
