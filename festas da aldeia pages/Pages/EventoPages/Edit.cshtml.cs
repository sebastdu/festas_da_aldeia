using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.EventoPages;

/// <summary>
/// Modelo da página de edição de dados de Eventos existentes.
/// Restrito a utilizadores com a função "Admin".
/// Realiza validações de sobreposição de eventos e trata conflitos de concorrência na base de dados.
/// </summary>
[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo de edição de eventos.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// O Evento a ser modificado, cujos valores são mapeados a partir do formulário (HTTP POST).
    /// </summary>
    [BindProperty]
    public Evento Evento { get; set; } = default!;

    /// <summary>
    /// Processa o pedido HTTP GET assincronamente para preencher o formulário de edição.
    /// Carrega as informações atuais do evento e preenche a dropdown de locais.
    /// </summary>
    /// <param name="id">O identificador único do evento a editar.</param>
    /// <returns>A página com o formulário carregado ou NotFound se o ID for nulo ou inexistente.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var evento = await _context.Eventos.FirstOrDefaultAsync(m => m.IdEvento == id);
        if (evento is null)
        {
            return NotFound();
        }
        Evento = evento;

        // Preenche a dropdown de seleção com o local atual pré-selecionado
        ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome", Evento.IdLocal);
        return Page();
    }

    /// <summary>
    /// Processa o pedido HTTP POST para submeter as alterações efetuadas no evento.
    /// Valida regras de integridade do local, datas de atuação e previne sobreposição com outros eventos.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice de eventos em caso de sucesso;
    /// recarrega a página atual com os respetivos erros de validação se necessário.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove as propriedades de navegação da validação, 
        // pois elas não são preenchidas no formulário de edição.
        ModelState.Remove("Evento.Local");
        ModelState.Remove("Evento.Cartazes");

        // Validação de negócio: verificar se o local selecionado existe na base de dados
        var localExists = await _context.Locais.AnyAsync(l => l.IdLocal == Evento.IdLocal);
        if (!localExists)
        {
            ModelState.AddModelError("Evento.IdLocal", "O local selecionado é inválido.");
        }

        // Validação de negócio: a data de fim deve ser posterior à data de início do evento
        if (Evento.DataFim <= Evento.DataInicio)
        {
            ModelState.AddModelError("Evento.DataFim", "A data de fim deve ser posterior à data de início.");
        }

        // Validação de negócio: evitar sobreposições de eventos no mesmo local (excluindo este próprio evento)
        if (localExists && Evento.DataFim > Evento.DataInicio)
        {
            bool hasOverlap = await _context.Eventos.AnyAsync(e =>
                e.IdLocal == Evento.IdLocal &&
                e.IdEvento != Evento.IdEvento &&
                Evento.DataInicio < e.DataFim &&
                Evento.DataFim > e.DataInicio);

            if (hasOverlap)
            {
                ModelState.AddModelError("Evento.IdLocal", "Já existe outro evento agendado para este local neste intervalo de tempo.");
            }
        }

        if (!ModelState.IsValid)
        {
            // Recarrega a dropdown para garantir a consistência visual no formulário
            ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome", Evento.IdLocal);
            return Page();
        }

        // Remove espaços supérfluos introduzidos pelo utilizador
        Evento.Nome = Evento.Nome.Trim();
        if (Evento.Descricao != null)
        {
            Evento.Descricao = Evento.Descricao.Trim();
        }
        if (Evento.Patrocinador != null)
        {
            Evento.Patrocinador = Evento.Patrocinador.Trim();
        }

        // Define o estado da entidade como modificado
        _context.Attach(Evento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Trata conflitos de concorrência onde o registo pode ter sido eliminado noutra sessão
            if (!EventoExists(Evento.IdEvento))
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

    /// <summary>
    /// Verifica se um evento com o identificador especificado existe na base de dados.
    /// </summary>
    /// <param name="id">O identificador único do evento.</param>
    /// <returns>True se existir, False caso contrário.</returns>
    private bool EventoExists(int id)
    {
        return _context.Eventos.Any(e => e.IdEvento == id);
    }
}
