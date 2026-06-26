using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;

namespace festas_da_aldeia_pages.Pages.EventoPages;

/// <summary>
/// Modelo da página de criação de novos Eventos.
/// Restrito a utilizadores com a função "Admin".
/// Realiza várias validações de integridade temporal e espacial (ex: sobreposição de eventos no mesmo local).
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo de criação de eventos.
    /// </summary>
    /// <param name="context">O contexto de base de dados da aplicação.</param>
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Processa o pedido HTTP GET para inicializar o formulário de criação.
    /// Preenche a dropdown de seleção do local.
    /// </summary>
    /// <returns>A página com o formulário carregado.</returns>
    public IActionResult OnGet()
    {
        // Cria a lista de opções para a dropdown. 
        // Guarda o "IdLocal" na BD, mas mostra o "Nome" do local ao utilizador.
        ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome");
        return Page();
    }

    /// <summary>
    /// Objeto que contém os dados do Evento a ser criado, mapeado a partir do formulário (HTTP POST).
    /// </summary>
    [BindProperty]
    public Evento Evento { get; set; } = default!;

    /// <summary>
    /// Processa o pedido HTTP POST para submissão do novo evento.
    /// Realiza validações de negócio, incluindo datas válidas e prevenção de sobreposição de eventos num mesmo local.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice administrativo em caso de sucesso;
    /// recarrega a página com mensagens de erro caso existam falhas de validação.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove as propriedades de navegação da validação, 
        // pois elas não são preenchidas no formulário de criação.
        ModelState.Remove("Evento.Local");
        ModelState.Remove("Evento.Cartazes");

        // Validação de negócio: verificar se o local selecionado existe na base de dados
        var localExists = await _context.Locais.AnyAsync(l => l.IdLocal == Evento.IdLocal);
        if (!localExists)
        {
            ModelState.AddModelError("Evento.IdLocal", "O local selecionado é inválido.");
        }

        // Validação de negócio: a data de início não pode ser no passado
        if (Evento.DataInicio < DateTime.Now)
        {
            ModelState.AddModelError("Evento.DataInicio", "A data de início deve ser no futuro.");
        }

        // Validação de negócio: a data de fim deve ser posterior à data de início
        if (Evento.DataFim <= Evento.DataInicio)
        {
            ModelState.AddModelError("Evento.DataFim", "A data de fim deve ser posterior à data de início.");
        }

        // Validação de negócio: evitar sobreposições de eventos no mesmo local ao mesmo tempo
        if (localExists && Evento.DataFim > Evento.DataInicio)
        {
            bool hasOverlap = await _context.Eventos.AnyAsync(e =>
                e.IdLocal == Evento.IdLocal &&
                Evento.DataInicio < e.DataFim &&
                Evento.DataFim > e.DataInicio);

            if (hasOverlap)
            {
                ModelState.AddModelError("Evento.IdLocal", "Já existe outro evento agendado para este local neste intervalo de tempo.");
            }
        }

        // Se a validação geral ou as validações de negócio falharem, recarrega o formulário
        if (!ModelState.IsValid)
        {
            // É necessário preencher novamente a dropdown para evitar erros na renderização do HTML
            ViewData["IdLocal"] = new SelectList(_context.Locais, "IdLocal", "Nome");
            return Page();
        }

        // Limpa espaços em branco supérfluos introduzidos pelo utilizador
        Evento.Nome = Evento.Nome.Trim();
        if (Evento.Descricao != null)
        {
            Evento.Descricao = Evento.Descricao.Trim();
        }
        if (Evento.Patrocinador != null)
        {
            Evento.Patrocinador = Evento.Patrocinador.Trim();
        }

        // Regista o evento e grava as alterações
        _context.Eventos.Add(Evento);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
