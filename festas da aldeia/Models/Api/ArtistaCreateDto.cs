using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    public class ArtistaCreateDto
    {
        [Required(ErrorMessage = "O Nome Artístico é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A Biografia deve ter um máximo de {1} caracteres.")]
        public string Biografia { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "O Contacto deve ter um máximo de {1} caracteres.")]
        [Phone(ErrorMessage = "O Contacto deve ser um número de telefone válido.")]
        public string Contacto { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "O link deve ter um máximo de {1} caracteres.")]
        [Url(ErrorMessage = "O link da foto de perfil deve ser uma URL válida.")]
        public string LinkFotoPerfil { get; set; } = string.Empty;
    }
}
