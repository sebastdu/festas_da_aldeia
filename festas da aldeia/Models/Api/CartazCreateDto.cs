using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    public class CartazCreateDto
    {
        [Required(ErrorMessage = "A Data e Hora da Atuação é de preenchimento obrigatório.")]
        public DateTime DataHoraAtuacao { get; set; }

        [Range(1, 480, ErrorMessage = "A duração deve estar entre 1 e 480 minutos.")]
        public int DuracaoMinutos { get; set; }

        [Required(ErrorMessage = "O Evento é de preenchimento obrigatório.")]
        public int IdEvento { get; set; }

        [Required(ErrorMessage = "O Artista é de preenchimento obrigatório.")]
        public int IdArtista { get; set; }
    }
}
