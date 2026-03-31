using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models
{
    /// <summary>
    /// Local onde os eventos podem acontecer
    /// </summary>
    public class Local
    {
        /// <summary>
        /// Id sequencial do local
        /// </summary>
        [Key]
        public int IdLocal { get; set; }

        /// <summary>
        /// Nome do local
        /// </summary>
        [Display(Name = "Nome do Local")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O {0} deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do local
        /// </summary>
        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A {0} deve ter um máximo de {1} caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o local é exterior ou interior
        /// </summary>
        [Display(Name = "Local Exterior")]
        public bool Outside { get; set; }

        /// <summary>
        /// Coordenadas para iframe (formato: latitude,longitude ou URL de mapa)
        /// </summary>
        [Display(Name = "Coordenadas / Mapa")]
        [StringLength(500, ErrorMessage = "As {0} devem ter um máximo de {1} caracteres.")]
        [RegularExpression(@"^(-?\d+(\.\d+)?),(-?\d+(\.\d+)?)$|^https?:\/\/.+", 
            ErrorMessage = "As coordenadas devem estar no formato latitude,longitude ou ser uma URL válida.")]
        public string Coordenadas { get; set; } = string.Empty;

        /*  ************************************** 
        *  Relationships
        *  ************************************** */

        /// <summary>
        /// Eventos que ocorrem neste local
        /// </summary>
        public ICollection<Evento> Eventos { get; set; } = [];
    }
}
