namespace festas_da_aldeia.Models.Api
{
    public class EventoDto
    {
        public int IdEvento { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Patrocinador { get; set; } = string.Empty;
        public int IdLocal { get; set; }
        public LocalDto? Local { get; set; }
        public List<CartazDto> Cartazes { get; set; } = [];
    }
}
