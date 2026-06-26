using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace festas_da_aldeia_pages.Pages
{
    /// <summary>
    /// Modelo da página de erro global da aplicação RallyFestas.
    /// Exibe detalhes de exceções capturadas e suporta códigos de status HTTP customizados.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// O identificador único do pedido HTTP que causou o erro (RequestId).
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indica se o identificador do pedido deve ser apresentado na vista.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// O código de estado HTTP original associado à falha (ex: 404, 500).
        /// </summary>
        public int? OriginalStatusCode { get; set; }

        /// <summary>
        /// Processa o pedido HTTP GET ou POST de erro.
        /// Captura o identificador da atividade e o código de estado HTTP original.
        /// </summary>
        /// <param name="statusCode">O código de estado HTTP de erro opcional.</param>
        public void OnGet(int? statusCode)
        {
            OriginalStatusCode = statusCode;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
