namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para representação de Locais na API.
    /// Utilizado para enviar as coordenadas e descrição dos recintos aos clientes da API.
    /// </summary>
    public class LocalDto
    {
        /// <summary>
        /// Identificador único do local.
        /// </summary>
        public int IdLocal { get; set; }

        /// <summary>
        /// Nome do local (ex: recinto, praça, palco).
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição das infraestruturas e acessos do local.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o local é ao ar livre (true) ou coberto/fechado (false).
        /// </summary>
        public bool Outside { get; set; }

        /// <summary>
        /// Coordenadas de GPS em formato de texto (Latitude,Longitude).
        /// </summary>
        public string Coordenadas { get; set; } = string.Empty;
    }
}
