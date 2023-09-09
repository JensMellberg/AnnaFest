using Microsoft.AspNetCore.Http;
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

        public string UserNameJson { get; set; }

		public string CssOverride { get; set; }

        protected void SetViewData()
		{
            if (this.User.Exists)
			{
				ViewData["User"] = this.User.Name;
                this.UserNameJson = JsonUtils.GetJavascriptText("userName", this.User.Name);
            }

			if (this.User.IsAdmin)
			{
				ViewData["IsAdmin"] = "yes";
            }

            if (this.session.Keys.Contains("cssOverride"))
            {
                ViewData["CssOverride"] = this.session.GetString("cssOverride");
            }
			else
			{
				ViewData["CssOverride"] = string.Empty;
			}

            var hideMenu = this.session.GetInt32("menuState") == 0;
            if (hideMenu)
			{
				ViewData["HideMenu"] = "yes";
			}

			var alertMessage = this.session.GetString("AlertMessage");
            if (!string.IsNullOrEmpty(alertMessage))
			{
                this.session.SetString("AlertMessage", "");
				ViewData["AlertMessage"] = alertMessage;
            }
		}

		protected void SetForwardAlert(string message)
		{
			this.session.SetString("AlertMessage", message);
		}
	}
}