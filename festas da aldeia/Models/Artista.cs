using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models
{
    /// <summary>
    /// Artista que pode atuar em eventos
    /// </summary>
    public class Artista
    {
        /// <summary>
        /// Id sequencial do artista
        /// </summary>
        [Key]
        public int IdArtista { get; set; }

        /// <summary>
        /// Nome artístico
        /// </summary>
        [Display(Name = "Nome Artístico")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O {0} deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Biografia ou descrição do artista
        /// </summary>
        [Display(Name = "Biografia")]
        [StringLength(500, ErrorMessage = "A {0} deve ter um máximo de {1} caracteres.")]
        public string Biografia { get; set; } = string.Empty;

        /// <summary>
        /// Contacto do artista (suporta formatos com +, espaços, etc.)
        /// </summary>
        [Display(Name = "Contacto")]
        [StringLength(20, ErrorMessage = "O {0} deve ter um máximo de {1} caracteres.")]
        [Phone(ErrorMessage = "O {0} deve ser um número de telefone válido.")]
        public string Contacto { get; set; } = string.Empty;

        /// <summary>
        /// Link para foto de perfil do artista
        /// </summary>
        [Display(Name = "Foto de Perfil")]
        [StringLength(500, ErrorMessage = "O link deve ter um máximo de {1} caracteres.")]
        [Url(ErrorMessage = "O link da foto de perfil deve ser uma URL válida.")]
        public string LinkFotoPerfil { get; set; } = string.Empty;

        /*  ************************************** 
        *  Relationships
        *  ************************************** */

        /// <summary>
        /// Eventos em que o artista atua
        /// </summary>
        public ICollection<Cartaz> Cartazes { get; set; } = [];
    }
}
