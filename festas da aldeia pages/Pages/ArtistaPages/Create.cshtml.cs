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
/// Modelo da página de criação de um novo Artista.
/// Permite que utilizadores autorizados com perfil de Administrador criem registos de novos artistas,
/// suportando o envio de ficheiros de imagem e validação de dados de perfil.
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    /// <summary>
    /// Contexto de acesso à base de dados, injetado via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do modelo da página de criação de artistas.
    /// </summary>
    /// <param name="context">O contexto da base de dados da aplicação.</param>
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Apresenta o formulário de criação de artista quando o pedido é efetuado via HTTP GET.
    /// </summary>
    /// <returns>A página que renderiza o formulário de criação.</returns>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// Objeto contendo os dados do Artista, vinculado aos campos do formulário.
    /// </summary>
    [BindProperty]
    public Artista Artista { get; set; } = default!;

    /// <summary>
    /// Ficheiro de imagem submetido pelo utilizador para a foto de perfil.
    /// Campo opcional que, quando preenchido, sobrepõe-se à inserção manual de links.
    /// </summary>
    [BindProperty]
    public IFormFile? FotoUpload { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    /// <summary>
    /// Processa a submissão do formulário de criação de artista via HTTP POST.
    /// Efetua validações do ficheiro de imagem se enviado, converte campos nulos para strings vazias,
    /// persiste o registo no banco de dados e redireciona para a lista.
    /// </summary>
    /// <returns>
    /// Redireciona para o índice administrativo em caso de sucesso;
    /// caso contrário, devolve controlo à mesma vista contendo os erros acumulados no ModelState.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Se houver uma imagem carregada pelo utilizador, procede-se ao upload
        if (FotoUpload != null)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(FotoUpload.FileName).ToLowerInvariant();

            // Valida se o formato do ficheiro coincide com extensões de imagem suportadas
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("FotoUpload", "Apenas são permitidas imagens com as extensões .jpg, .jpeg, .png, .gif ou .webp.");
            }

            // Impede a submissão de imagens excessivamente pesadas (limite de 5MB) para gerir o armazenamento do servidor
            if (FotoUpload.Length > 5242880)
            {
                ModelState.AddModelError("FotoUpload", "O tamanho da imagem não pode exceder 5 MB.");
            }

            if (ModelState.IsValid)
            {
                // Gera um nome aleatório único (GUID) para o ficheiro para evitar colisões ou substituições acidentais no disco
                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "artistas");

                // Assegura a criação física da pasta de destino na diretoria wwwroot se esta ainda não existir
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var filePath = Path.Combine(uploadDir, uniqueFileName);

                // Grava assincronamente a imagem submetida no caminho do servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await FotoUpload.CopyToAsync(fileStream);
                }

                // Associa o caminho local relativo da imagem à propriedade correspondente do modelo
                Artista.LinkFotoPerfil = "/images/artistas/" + uniqueFileName;
            }
        }

        // Se existirem erros de validação detetados, devolve controlo à página com as mensagens de erro
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Preenche campos opcionais com strings vazias para respeitar restrições NOT NULL da base de dados sem requerer migrações
        Artista.Biografia ??= string.Empty;
        Artista.LinkFotoPerfil ??= string.Empty;

        // Adiciona e persiste o novo objeto Artista na base de dados relacional
        _context.Artistas.Add(Artista);
        await _context.SaveChangesAsync();

        // Redireciona o utilizador de volta para a lista geral de artistas
        return RedirectToPage("./Index");
    }
}
