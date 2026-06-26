using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace festas_da_aldeia_pages.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int? OriginalStatusCode { get; set; }

        public void OnGet(int? statusCode)
        {
            OriginalStatusCode = statusCode;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

}
