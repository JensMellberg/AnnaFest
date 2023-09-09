using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace AnnaFest.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	[IgnoreAntiforgeryToken]
	public class ErrorModel : PageModel
	{
		public string ErrorMessage { get; set; }

		public string StackTrace { get; set; }

		public ErrorModel()
		{
		}

		public void OnGet()
		{
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			this.ErrorMessage = exceptionHandlerPathFeature?.Error?.Message ?? string.Empty;
			this.StackTrace = exceptionHandlerPathFeature?.Error?.StackTrace ?? string.Empty;
        }
	}
}