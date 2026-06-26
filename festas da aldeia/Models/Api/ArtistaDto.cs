namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para representação de Artistas na API.
    /// Utilizado para envio de informação de artistas para os clientes da API.
    /// </summary>
    public class ArtistaDto
    {
        /// <summary>
        /// Identificador único do artista.
        /// </summary>
        public int IdArtista { get; set; }

        /// <summary>
        /// Nome artístico do artista.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Biografia do artista (descrição da sua carreira ou estilo).
        /// </summary>
        public string Biografia { get; set; } = string.Empty;

        /// <summary>
        /// Contacto telefónico ou eletrónico do artista.
        /// </summary>
        public string Contacto { get; set; } = string.Empty;

        /// <summary>
        /// Link de acesso ou caminho relativo para a foto de perfil do artista.
        /// </summary>
        public string LinkFotoPerfil { get; set; } = string.Empty;
    }
}
