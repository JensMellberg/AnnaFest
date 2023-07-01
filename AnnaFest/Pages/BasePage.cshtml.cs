using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    public class BasePage : PageModel
	{
		protected readonly IWebHostEnvironment environment;
		protected readonly ISession session;
		protected readonly IUser User;

		public BasePage(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
		{
			this.session = httpContextAccessor.HttpContext.Session;
			this.environment = environment;
			this.User = AnnaFest.User.Current(this.session);

        }

		protected void SetViewData()
		{
            if (this.User.Exists)
			{
				ViewData["User"] = this.User.Name;
			}

			if (this.User.IsAdmin)
			{
				ViewData["IsAdmin"] = "yes";
            }
		}
	}
}