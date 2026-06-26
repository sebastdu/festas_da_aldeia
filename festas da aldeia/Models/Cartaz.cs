using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace festas_da_aldeia.Models
{
    /// <summary>
    /// Representa a entidade Cartaz no sistema RallyFestas.
    /// Esta classe funciona como tabela de associação (muitos-para-muitos) entre as entidades Artista e Evento,
    /// registando os dados específicos de cada atuação, tais como a data, hora de início e a duração prevista em minutos.
    /// </summary>
    public class Cartaz
    {
        /// <summary>
        /// Id sequencial do cartaz
        /// </summary>
        [Key]
        public int IdCartaz { get; set; }

        /// <summary>
        /// Data e hora da atuação
        /// </summary>
        [Display(Name = "Data e Hora da Atuação")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        public DateTime DataHoraAtuacao { get; set; }

        /// <summary>
        /// Duração da atuação em minutos
        /// </summary>
        [Display(Name = "Duração (minutos)")]
        [Range(1, 480, ErrorMessage = "A duração deve estar entre 1 e 480 minutos.")]
        public int DuracaoMinutos { get; set; }

        /*  ************************************** 
        *  Relationships
        *  ************************************** */

        /// <summary>
        /// Id do evento
        /// </summary>
        [Display(Name = "Evento")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [ForeignKey(nameof(Evento))]
        public int IdEvento { get; set; }

        /// <summary>
        /// Evento associado
        /// </summary>
        public Evento Evento { get; set; } = null!;

        /// <summary>
        /// Id do artista
        /// </summary>
        [Display(Name = "Artista")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [ForeignKey(nameof(Artista))]
        public int IdArtista { get; set; }

        /// <summary>
        /// Artista associado
        /// </summary>
        public Artista Artista { get; set; } = null!;
    }
}
