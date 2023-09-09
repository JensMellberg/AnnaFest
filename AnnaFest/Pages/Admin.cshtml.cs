using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    [IgnoreAntiforgeryToken]
    public class AdminModel : BasePage
    {

        public AdminModel(IWebHostEnvironment environment, IHttpContextAccessor session) : base(environment, session)
        {
        }

        public string UploadEnabledText => SettingsRepository.Instance.UploadEnabled ? "Blockera bilduppladdning" : "Tillåt bilduppladdning";

        public IActionResult OnGet()
        {
            if (!this.User.IsAdmin)
            {
                return RedirectToPage("/Index");
            }

            this.SetViewData();
            return Page();
        }

        public IActionResult OnPost(string action, string css)
        {
            if (!this.User.IsAdmin)
            {
                return new JsonResult("");
            }

            if (action == "download")
            {
                var memoryStream = new MemoryStream();
                PhotoRepository.Instance.DownloadZip(environment.WebRootPath, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(memoryStream, "application/octet-stream");
            }
            else if (action == "toggleUpload")
            {
                var instance = SettingsRepository.Instance;
                instance.UploadEnabled = !instance.UploadEnabled;
                return new JsonResult(new { buttonText = this.UploadEnabledText });
            }
            else if (action == "cssOverride")
            {
                this.session.SetString("cssOverride", css ?? string.Empty);
            }

            return new JsonResult("");
        }
    }
}
