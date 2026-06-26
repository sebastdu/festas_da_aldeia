using System.ComponentModel.DataAnnotations;

namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para criação ou edição de Locais via API.
    /// Valida o nome, a descrição, a cobertura e o formato das coordenadas geográficas submetidos.
    /// </summary>
    public class LocalCreateDto
    {
        /// <summary>
        /// Nome do local. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O Nome é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter um máximo de {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do recinto físico.
        /// </summary>
        [StringLength(500, ErrorMessage = "A Descrição deve ter um máximo de {1} caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o recinto é ao ar livre (true) ou coberto (false).
        /// </summary>
        public bool Outside { get; set; }

        /// <summary>
        /// Coordenadas geográficas ou link de mapa.
        /// Deve estar no formato 'latitude,longitude' ou ser uma URL HTTP/HTTPS válida.
        /// </summary>
        [StringLength(500, ErrorMessage = "As Coordenadas devem ter um máximo de {1} caracteres.")]
        [RegularExpression(@"^(-?\d+(\.\d+)?),(-?\d+(\.\d+)?)$|^https?:\/\/.+", 
            ErrorMessage = "As coordenadas devem estar no formato latitude,longitude ou ser uma URL válida.")]
        public string Coordenadas { get; set; } = string.Empty;
    }
}
