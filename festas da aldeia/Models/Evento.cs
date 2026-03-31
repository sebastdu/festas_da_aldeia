using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace festas_da_aldeia.Models
{
    /// <summary>
    /// Evento que ocorre no âmbito das festas da aldeia
    /// </summary>
    public class Evento
    {
        /// <summary>
        /// Id sequencial do evento
        /// </summary>
        [Key]
        public int IdEvento { get; set; }

        /// <summary>
        /// Nome do evento
        /// </summary>
        [Display(Name = "Nome do Evento")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O {0} deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do evento
        /// </summary>
        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A {0} deve ter um máximo de {1} caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de início do evento
        /// </summary>
        [Display(Name = "Data de Início")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data e hora de fim do evento
        /// </summary>
        [Display(Name = "Data de Fim")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Nome do patrocinador do evento
        /// </summary>
        [Display(Name = "Patrocinador")]
        [StringLength(100, ErrorMessage = "O {0} deve ter um máximo de {1} caracteres.")]
        public string Patrocinador { get; set; } = string.Empty;

        /*  ************************************** 
        *  Relationships
        *  ************************************** */

        /// <summary>
        /// Id do local onde o evento ocorre
        /// </summary>
        [Display(Name = "Local")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [ForeignKey(nameof(Local))]
        public int IdLocal { get; set; }

        /// <summary>
        /// Local onde o evento ocorre
        /// </summary>
        public Local Local { get; set; } = null!;

        /// <summary>
        /// Artistas que atuam neste evento
        /// </summary>
        public ICollection<Cartaz> Cartazes { get; set; } = [];
    }
}
