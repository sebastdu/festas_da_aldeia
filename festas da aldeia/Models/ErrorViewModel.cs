namespace festas_da_aldeia.Models
{
    /// <summary>
    /// Modelo de dados utilizado para apresentar informações detalhadas sobre erros no portal.
    /// Geralmente usado pelas vistas MVC/Razor Pages para expor o identificador da requisição em falha.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// O identificador único do pedido HTTP falhado.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indica se o identificador do pedido deve ser exibido.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
