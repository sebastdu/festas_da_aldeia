namespace festas_da_aldeia.Models.Api
{
    public class ArtistaDto
    {
        public int IdArtista { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Biografia { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
        public string LinkFotoPerfil { get; set; } = string.Empty;
    }
}
