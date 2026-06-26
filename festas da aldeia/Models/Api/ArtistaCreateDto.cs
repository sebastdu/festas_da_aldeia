using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para criação ou edição de Artistas via API.
    /// Contém as anotações de validação de dados necessárias para submissão na API.
    /// </summary>
    public class ArtistaCreateDto
    {
        /// <summary>
        /// Nome artístico do artista. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O Nome Artístico é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Biografia resumida do artista.
        /// </summary>
        [StringLength(500, ErrorMessage = "A Biografia deve ter um máximo de {1} caracteres.")]
        public string Biografia { get; set; } = string.Empty;

        /// <summary>
        /// Número de telefone de contacto. Deve ser um número de telefone válido.
        /// </summary>
        [StringLength(20, ErrorMessage = "O Contacto deve ter um máximo de {1} caracteres.")]
        [Phone(ErrorMessage = "O Contacto deve ser um número de telefone válido.")]
        public string Contacto { get; set; } = string.Empty;

        /// <summary>
        /// URL pública da imagem de perfil. Deve ser um link válido.
        /// </summary>
        [StringLength(500, ErrorMessage = "O link deve ter um máximo de {1} caracteres.")]
        [Url(ErrorMessage = "O link da foto de perfil deve ser uma URL válida.")]
        public string LinkFotoPerfil { get; set; } = string.Empty;
    }
}
