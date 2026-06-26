using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para criação ou edição de Atuações (Cartaz) via API.
    /// Define as regras de validação para novos agendamentos de atuações de artistas.
    /// </summary>
    public class CartazCreateDto
    {
        /// <summary>
        /// Data e hora de agendamento do início da atuação. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "A Data e Hora da Atuação é de preenchimento obrigatório.")]
        public DateTime DataHoraAtuacao { get; set; }

        /// <summary>
        /// Duração estimada da atuação em minutos (limite de 1 a 480 minutos).
        /// </summary>
        [Range(1, 480, ErrorMessage = "A duração deve estar entre 1 e 480 minutos.")]
        public int DuracaoMinutos { get; set; }

        /// <summary>
        /// Identificador único do evento ao qual esta atuação pertence. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O Evento é de preenchimento obrigatório.")]
        public int IdEvento { get; set; }

        /// <summary>
        /// Identificador único do artista que irá atuar. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O Artista é de preenchimento obrigatório.")]
        public int IdArtista { get; set; }
    }
}
