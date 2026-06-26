namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para representação de Atuações (Cartaz) na API.
    /// Contém informações sobre o agendamento, duração e as entidades associadas (Evento e Artista).
    /// </summary>
    public class CartazDto
    {
        /// <summary>
        /// Identificador único da atuação no cartaz.
        /// </summary>
        public int IdCartaz { get; set; }

        /// <summary>
        /// Data e hora agendadas para o início da atuação.
        /// </summary>
        public DateTime DataHoraAtuacao { get; set; }

        /// <summary>
        /// Duração prevista da atuação em minutos.
        /// </summary>
        public int DuracaoMinutos { get; set; }

        /// <summary>
        /// Identificador único do evento associado a esta atuação.
        /// </summary>
        public int IdEvento { get; set; }

        /// <summary>
        /// Identificador único do artista associado a esta atuação.
        /// </summary>
        public int IdArtista { get; set; }

        /// <summary>
        /// Dados simplificados do Evento (DTO). Opcional.
        /// </summary>
        public EventoDto? Evento { get; set; }

        /// <summary>
        /// Dados simplificados do Artista (DTO). Opcional.
        /// </summary>
        public ArtistaDto? Artista { get; set; }
    }
}
