using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;
using festas_da_aldeia.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace festas_da_aldeia_pages.Pages.ArtistaPages;

/// <summary>
/// Modelo da página de edição de um Artista existente.
/// Permite que utilizadores com perfil de Administrador modifiquem os dados de um artista,
/// incluindo a substituição ou remoção da sua foto de perfil anterior.
/// </summary>
[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de edição de artistas.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Objeto que contém os dados do Artista a ser editado.
    /// </summary>
    [BindProperty]
    public Artista Artista { get; set; } = default!;

    /// <summary>
    /// Ficheiro de imagem carregado para substituir a foto de perfil atual do artista.
    /// </summary>
    [BindProperty]
    public IFormFile? FotoUpload { get; set; }

    /// <summary>
    /// Carrega os dados do artista especificado pelo ID quando a página é acedida via HTTP GET.
    /// </summary>
    /// <param name="id">O identificador único do artista a ser editado.</param>
    /// <returns>A página com o formulário de edição preenchido ou erro 404 (NotFound) se não existir.</returns>
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
        Artista = artista;
        return Page();
    }

    /// <summary>
    /// Processa a submissão do formulário de edição via HTTP POST.
    /// Executa as validações do ficheiro de imagem se carregado, remove a imagem antiga se aplicável
    /// para evitar ficheiros órfãos, atualiza a entidade no banco de dados e trata concorrência.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice administrativo em caso de sucesso;
    /// caso contrário, devolve controlo à mesma vista contendo os erros acumulados no ModelState.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Se houver uma nova imagem carregada pelo utilizador para substituição
        if (FotoUpload != null)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(FotoUpload.FileName).ToLowerInvariant();

            // Valida se o formato do ficheiro coincide com extensões de imagem comuns
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("FotoUpload", "Apenas são permitidas imagens com as extensões .jpg, .jpeg, .png, .gif ou .webp.");
            }

            // Impede o envio de ficheiros excessivamente grandes (limite de 5MB)
            if (FotoUpload.Length > 5242880)
            {
                ModelState.AddModelError("FotoUpload", "O tamanho da imagem não pode exceder 5 MB.");
            }

            if (ModelState.IsValid)
            {
                // Obtém o estado atual da entidade na base de dados para podermos apagar a imagem antiga associada
                var oldArtista = await _context.Artistas.AsNoTracking().FirstOrDefaultAsync(a => a.IdArtista == Artista.IdArtista);

                // Gera um nome aleatório único (GUID) para o novo ficheiro
                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "artistas");

                // Assegura que o diretório físico existe no servidor
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var filePath = Path.Combine(uploadDir, uniqueFileName);

                // Grava assincronamente a nova imagem no disco rígido do servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await FotoUpload.CopyToAsync(fileStream);
                }

                // Se existia uma imagem local antiga guardada na pasta 'wwwroot/images/artistas/',
                // procede-se à sua eliminação física para evitar ficheiros acumulados sem utilidade no servidor
                if (oldArtista != null && !string.IsNullOrEmpty(oldArtista.LinkFotoPerfil) && oldArtista.LinkFotoPerfil.StartsWith("/images/artistas/"))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldArtista.LinkFotoPerfil.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (Exception)
                        {
                            // Ignora a exceção se o ficheiro estiver bloqueado ou inacessível no sistema de ficheiros
                        }
                    }
                }

                // Atualiza o caminho relativo no modelo com o novo ficheiro carregado
                Artista.LinkFotoPerfil = "/images/artistas/" + uniqueFileName;
            }
        }

        // Se existirem erros de validação nos dados submetidos, interrompe e recarrega a página
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Garante que campos não preenchidos fiquem como strings vazias, respeitando a restrição NOT NULL na base de dados
        Artista.Biografia ??= string.Empty;
        Artista.LinkFotoPerfil ??= string.Empty;

        // Marca o estado da entidade como Modificado para que o EF Core gere o comando UPDATE correspondente
        _context.Attach(Artista).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Trata eventuais conflitos de concorrência se o registo tiver sido removido em simultâneo
            if (!ArtistaExists(Artista.IdArtista))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        // Redireciona o utilizador de volta para a lista geral de artistas
        return RedirectToPage("./Index");
    }

    /// <summary>
    /// Verifica se um determinado artista existe na base de dados pelo seu ID único.
    /// </summary>
    /// <param name="id">O identificador do artista a pesquisar.</param>
    /// <returns>True se o artista existir; caso contrário, False.</returns>
    private bool ArtistaExists(int id)
    {
        return _context.Artistas.Any(e => e.IdArtista == id);
    }
}
