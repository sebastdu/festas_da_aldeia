namespace festas_da_aldeia.Models.Api
{
    public class CartazDto
    {
        public int IdCartaz { get; set; }
        public DateTime DataHoraAtuacao { get; set; }
        public int DuracaoMinutos { get; set; }
        public int IdEvento { get; set; }
        public int IdArtista { get; set; }
        public EventoDto? Evento { get; set; }
        public ArtistaDto? Artista { get; set; }
    }
}
