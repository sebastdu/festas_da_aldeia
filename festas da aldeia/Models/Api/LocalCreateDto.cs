using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    public class LocalCreateDto
    {
        [Required(ErrorMessage = "O Nome é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A Descrição deve ter um máximo de {1} caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        public bool Outside { get; set; }

        [StringLength(500, ErrorMessage = "As Coordenadas devem ter um máximo de {1} caracteres.")]
        [RegularExpression(@"^(-?\d+(\.\d+)?),(-?\d+(\.\d+)?)$|^https?:\/\/.+", 
            ErrorMessage = "As coordenadas devem estar no formato latitude,longitude ou ser uma URL válida.")]
        public string Coordenadas { get; set; } = string.Empty;
    }
}
