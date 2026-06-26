using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models
{
    /// <summary>
    /// Representa a entidade Artista no sistema RallyFestas.
    /// Esta classe é encarregue de modelar os artistas (cantores, bandas, ranchos folclóricos) 
    /// que atuam nas festividades, mantendo os seus dados pessoais, contactos, biografia e foto de perfil.
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
        public string? Biografia { get; set; }

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
        public string? LinkFotoPerfil { get; set; }

        /*  ************************************** 
        *  Relationships
        *  ************************************** */

        /// <summary>
        /// Eventos em que o artista atua
        /// </summary>
        public ICollection<Cartaz> Cartazes { get; set; } = [];
    }
}
