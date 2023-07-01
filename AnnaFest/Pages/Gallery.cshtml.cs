using Microsoft.AspNetCore.Mvc;

namespace AnnaFest.Pages
{
    [IgnoreAntiforgeryToken]
    public class PrivacyModel : BasePage
	{
		public IList<PhotoJsonModel> Photos { get; set; }

		public PrivacyModel(IWebHostEnvironment environment, IHttpContextAccessor session) : base(environment, session)
		{
		}

		public IActionResult OnGet()
		{
            if (!this.User.Exists)
            {
                return RedirectToPage("Login", new { returnTo = "Gallery" });
            }

            this.SetViewData();
            this.Photos = PhotoRepository.Instance.GetAllPhotos(this.environment.WebRootPath).Select(x => x.GetJsonModel()).ToList();
			return Page();
		}

        public JsonResult OnPost(string action, string id)
        {
            if (action == "delete")
            {
                PhotoRepository.Instance.DeletePhoto(Guid.Parse(id), this.environment.WebRootPath);
            }

            return new JsonResult("");
        }
    }
}