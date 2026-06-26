using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.CartazPages;

/// <summary>
/// Modelo da página de edição de uma atuação no Cartaz.
/// Permite a utilizadores com perfil de Administrador atualizar os detalhes (data, hora, duração)
/// de atuações de artistas, preenchendo as listas de seleção e tratando concorrência.
/// </summary>
[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de edição de cartazes.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto contendo os dados da atuação (Cartaz) a editar, vinculado aos campos do formulário na página.
    /// </summary>
    [BindProperty]
    public Cartaz Cartaz { get; set; } = default!;

    /// <summary>
    /// Coleção de seleção contendo todos os Artistas para preenchimento da dropdown correspondente.
    /// </summary>
    public SelectList ArtistasSelectList { get; set; } = default!;

    /// <summary>
    /// Coleção de seleção contendo todos os Eventos para preenchimento da dropdown correspondente.
    /// </summary>
    public SelectList EventosSelectList { get; set; } = default!;

    /// <summary>
    /// Carrega a atuação pretendida e inicializa as listas de seleção dos Artistas e Eventos (GET).
    /// </summary>
    /// <param name="id">O identificador único da atuação.</param>
    /// <returns>A página com o formulário preenchido ou erro 404 (NotFound).</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var cartaz = await _context.Cartazes.FirstOrDefaultAsync(m => m.IdCartaz == id);
        if (cartaz is null)
        {
            return NotFound();
        }
        Cartaz = cartaz;

        // Preenche as dropdown lists com dados da base de dados
        ArtistasSelectList = new SelectList(_context.Artistas, "IdArtista", "Nome");
        EventosSelectList = new SelectList(_context.Eventos, "IdEvento", "Nome");
        return Page();
    }

    // Para proteção contra ataques de sobreposição de campos, consulte https://aka.ms/RazorPagesCRUD.
    /// <summary>
    /// Processa a submissão dos dados editados da atuação via HTTP POST.
    /// Valida o modelo, anexa a entidade ao contexto para assinalar a sua modificação e persiste na base de dados.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice de atuações em caso de sucesso;
    /// caso contrário, recarrega a página atual preenchendo novamente as dropdowns.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove as entidades de navegação para evitar erros de validação causados por campos inexistentes no formulário
        ModelState.Remove("Cartaz.Artista");
        ModelState.Remove("Cartaz.Evento");

        if (!ModelState.IsValid)
        {
            ArtistasSelectList = new SelectList(_context.Artistas, "IdArtista", "Nome");
            EventosSelectList = new SelectList(_context.Eventos, "IdEvento", "Nome");
            return Page();
        }

        // Informa o contexto que a entidade foi modificada e precisa de ser atualizada na base de dados
        _context.Attach(Cartaz).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Trata conflitos de concorrência se o registo de atuação tiver sido apagado simultaneamente
            if (!CartazExists(Cartaz.IdCartaz))
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
    /// Verifica se uma atuação no cartaz existe na base de dados com base no ID fornecido.
    /// </summary>
    /// <param name="id">O ID da atuação a pesquisar.</param>
    /// <returns>True se a atuação existir; caso contrário, False.</returns>
    private bool CartazExists(int id)
    {
        return _context.Cartazes.Any(e => e.IdCartaz == id);
    }
}
