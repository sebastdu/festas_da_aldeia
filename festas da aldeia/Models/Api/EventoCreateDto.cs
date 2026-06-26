using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para criação ou edição de Eventos via API.
    /// Define as regras de validação aplicadas no servidor para os campos submetidos.
    /// </summary>
    public class EventoCreateDto
    {
        /// <summary>
        /// Nome do evento. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O Nome do Evento é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do evento (objetivo, público, etc.).
        /// </summary>
        [StringLength(500, ErrorMessage = "A Descrição deve ter um máximo de {1} caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de início do evento. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "A Data de Início é de preenchimento obrigatório.")]
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data e hora de encerramento do evento. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "A Data de Fim é de preenchimento obrigatório.")]
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Patrocinador oficial do evento.
        /// </summary>
        [StringLength(100, ErrorMessage = "O Patrocinador deve ter um máximo de {1} caracteres.")]
        public string Patrocinador { get; set; } = string.Empty;

        /// <summary>
        /// Identificador único do local geográfico associado. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O Local é de preenchimento obrigatório.")]
        public int IdLocal { get; set; }
    }
}
