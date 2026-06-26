using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.LocalPages;

/// <summary>
/// Modelo da página de edição de um Local existente.
/// Permite que utilizadores com funções de Administrador atualizem as informações de um recinto,
/// garantindo que o nome alterado não colide com outros locais e limpando espaços supérfluos.
/// </summary>
[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de edição de locais.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto que contém os dados do Local a editar, vinculado aos campos do formulário na página.
    /// </summary>
    [BindProperty]
    public Local Local { get; set; } = default!;

    /// <summary>
    /// Carrega as informações do local especificado por ID quando a página é acedida via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador do local a ser carregado.</param>
    /// <returns>A página com o formulário preenchido ou erro 404 (NotFound) se o local não for encontrado.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var local = await _context.Locais.FirstOrDefaultAsync(m => m.IdLocal == id);
        if (local is null)
        {
            return NotFound();
        }
        Local = local;
        return Page();
    }

    // Para proteção contra ataques de sobreposição de campos, consulte https://aka.ms/RazorPagesCRUD.
    /// <summary>
    /// Processa a submissão do formulário de edição de local via HTTP POST.
    /// Valida que o nome alterado não colide com outro local existente, limpa os dados textuais,
    /// atualiza o estado da entidade no contexto e grava as alterações.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice de locais em caso de sucesso;
    /// caso contrário, devolve controlo à mesma vista contendo os erros de validação.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove os Eventos da validação, visto que não são editados diretamente no formulário de local
        ModelState.Remove("Local.Eventos");

        // Validação customizada: garante que o nome editado não seja igual ao de outro local já existente
        if (!string.IsNullOrWhiteSpace(Local.Nome) &&
            await _context.Locais.AnyAsync(l => l.Nome.ToLower() == Local.Nome.Trim().ToLower() && l.IdLocal != Local.IdLocal))
        {
            ModelState.AddModelError("Local.Nome", "Já existe um local registado com este nome.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Limpa espaços em branco desnecessários nos dados
        Local.Nome = Local.Nome.Trim();
        if (Local.Descricao != null)
        {
            Local.Descricao = Local.Descricao.Trim();
        }
        if (Local.Coordenadas != null)
        {
            Local.Coordenadas = Local.Coordenadas.Trim();
        }

        // Assinala a entidade como modificada para que o EF Core atualize o registo correspondente
        _context.Attach(Local).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Trata eventuais conflitos se o local for eliminado por outro utilizador antes da gravação
            if (!LocalExists(Local.IdLocal))
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
    /// Verifica se um local existe na base de dados com base no ID fornecido.
    /// </summary>
    /// <param name="id">O ID do local a pesquisar.</param>
    /// <returns>True se o local existir; caso contrário, False.</returns>
    private bool LocalExists(int id)
    {
        return _context.Locais.Any(e => e.IdLocal == id);
    }
}
