using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    public class LaddaUppModel : BasePage
    {

        public LaddaUppModel(IWebHostEnvironment environment, IHttpContextAccessor session) : base(environment, session)
        {
        }

        public IActionResult OnGet()
        {
            if (!this.User.Exists)
            {
                return RedirectToPage("Login", new { returnTo = "LaddaUpp" });
            }

            this.SetViewData();
            return Page();
        }

        public IActionResult OnPost(string description, IFormFile postedFile)
        {
            description = JsonUtils.RemoveAllWeirdness(description);
            PhotoRepository.Instance.AddFile(this.environment.WebRootPath, postedFile, description, this.User.Name);
            return RedirectToPage("Index");
        }
    }
}
