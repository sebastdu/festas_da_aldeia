using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

/// <summary>
/// Modelo da página de eliminação de um Artista.
/// Permite que utilizadores com privilégios de Administrador eliminem o registo de um artista da base de dados.
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de eliminação de artistas.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto contendo os dados do Artista a ser eliminado, vinculado à página de confirmação.
    /// </summary>
    [BindProperty]
    public Artista Artista { get; set; } = default!;

    /// <summary>
    /// Carrega os dados do artista a eliminar com base no ID fornecido via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador do artista.</param>
    /// <returns>A página de confirmação de eliminação ou erro 404 (NotFound) se o artista não existir.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var artista = await _context.Artistas.FirstOrDefaultAsync(m => m.IdArtista == id);
        if (artista is null)
        {
            return NotFound();
        }
        else
        {
            Artista = artista;
        }

        return Page();
    }

    /// <summary>
    /// Processa a eliminação do artista via HTTP POST após confirmação do utilizador.
    /// Remove o registo correspondente da base de dados relacional e grava as alterações.
    /// </summary>
    /// <param name="id">O identificador do artista a remover.</param>
    /// <returns>Redireciona para o índice geral de artistas.</returns>
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var artista = await _context.Artistas.FindAsync(id);
        if (artista != null)
        {
            Artista = artista;

            // Remove fisicamente o registo de artista da base de dados
            _context.Artistas.Remove(Artista);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
