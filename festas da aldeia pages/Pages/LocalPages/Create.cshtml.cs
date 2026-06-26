using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;

namespace festas_da_aldeia_pages.Pages.LocalPages;

/// <summary>
/// Modelo da página de criação de um novo Local.
/// Permite que utilizadores com privilégios de Administrador criem registos de novos recintos ou palcos,
/// verificando se o nome do local é único e limpando os espaços desnecessários dos campos submetidos.
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de criação de locais.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Apresenta o formulário de criação de local quando o pedido é efetuado via HTTP GET.
    /// </summary>
    /// <returns>A página que renderiza o formulário de criação.</returns>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// Objeto contendo os dados do Local a ser criado, vinculado aos campos do formulário.
    /// </summary>
    [BindProperty]
    public Local Local { get; set; } = default!;

    /// <summary>
    /// Processa a submissão do formulário de criação de local via HTTP POST.
    /// Efetua validações personalizadas (ex: garantir que o nome do local seja único),
    /// limpa os dados textuais submetidos e persiste o registo no banco de dados.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice de locais em caso de sucesso;
    /// caso contrário, devolve controlo à mesma vista contendo os erros de validação acumulados.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove os Eventos da validação, visto que um local novo 
        // inicia sempre com uma lista vazia no formulário de criação.
        ModelState.Remove("Local.Eventos");

        // Validação customizada: Garante a unicidade do nome do local para evitar duplicações ambíguas
        if (!string.IsNullOrWhiteSpace(Local.Nome) &&
            await _context.Locais.AnyAsync(l => l.Nome.ToLower() == Local.Nome.Trim().ToLower()))
        {
            ModelState.AddModelError("Local.Nome", "Já existe um local registado com este nome.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Limpa espaços em branco supérfluos no início e fim dos dados inseridos pelo utilizador
        Local.Nome = Local.Nome.Trim();
        if (Local.Descricao != null)
        {
            Local.Descricao = Local.Descricao.Trim();
        }
        if (Local.Coordenadas != null)
        {
            Local.Coordenadas = Local.Coordenadas.Trim();
        }

        // Adiciona e persiste o novo local na base de dados
        _context.Locais.Add(Local);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
