namespace festas_da_aldeia.Models.Api
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para representação de Eventos na API.
    /// Reúne os dados do evento, o local físico e a lista de atuações escaladas.
    /// </summary>
    public class EventoDto
    {
        /// <summary>
        /// Identificador único do evento.
        /// </summary>
        public int IdEvento { get; set; }

        /// <summary>
        /// Nome do evento.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do evento.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de início oficial do evento.
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data e hora de conclusão prevista do evento.
        /// </summary>
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Nome do patrocinador oficial associado ao evento.
        /// </summary>
        public string Patrocinador { get; set; } = string.Empty;

        /// <summary>
        /// Identificador único do local geográfico do evento.
        /// </summary>
        public int IdLocal { get; set; }

        /// <summary>
        /// Dados simplificados do Local (DTO). Opcional.
        /// </summary>
        public LocalDto? Local { get; set; }

        /// <summary>
        /// Lista contendo as atuações de artistas agendadas no cartaz deste evento.
        /// </summary>
        public List<CartazDto> Cartazes { get; set; } = [];
    }
}
