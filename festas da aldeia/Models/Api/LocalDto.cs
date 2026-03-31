namespace festas_da_aldeia.Models.Api
{
    public class LocalDto
    {
        public int IdLocal { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Outside { get; set; }
        public string Coordenadas { get; set; } = string.Empty;
    }
}
