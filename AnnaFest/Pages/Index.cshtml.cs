using AnnaFest.Quiz;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace AnnaFest.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : BasePage
	{

        public IndexModel(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : base(environment, httpContextAccessor)
        {
        }

        public string PhotoJson { get; set; }

		public void OnGet()
		{
            this.SetViewData();
			this.PhotoJson = JsonUtils.GetJavascriptText("photoJson", this.GetPhotoJsonModel());
		}

		public JsonResult OnPost(string action, int value)
		{
            if (action == "changeMenu")
            {
                this.session.SetInt32("menuState", value);
                return new JsonResult("");
            }

			return new JsonResult(this.GetPhotoJsonModel());
        }

		private object GetPhotoJsonModel()
		{
            var currentPhoto = PhotoRepository.Instance.GetCurrentPhoto(this.environment.WebRootPath);
			if (currentPhoto == null)
			{
				return null;
			}

			return currentPhoto.GetJsonModel();
        }
	}
}