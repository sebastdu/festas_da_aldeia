using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    public class EventoCreateDto
    {
        [Required(ErrorMessage = "O Nome do Evento é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A Descrição deve ter um máximo de {1} caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Data de Início é de preenchimento obrigatório.")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "A Data de Fim é de preenchimento obrigatório.")]
        public DateTime DataFim { get; set; }

        [StringLength(100, ErrorMessage = "O Patrocinador deve ter um máximo de {1} caracteres.")]
        public string Patrocinador { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Local é de preenchimento obrigatório.")]
        public int IdLocal { get; set; }
    }
}
