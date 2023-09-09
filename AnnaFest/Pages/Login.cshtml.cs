using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    public class LoginModel : PageModel
    {
        public string ReturnTo { get; set; }

        private ISession session;

        public LoginModel(IHttpContextAccessor session)
        {
            this.session = session.HttpContext.Session;
        }

        public void OnGet(string returnTo)
        {
            if (!string.IsNullOrEmpty(returnTo))
            {
                this.session.SetString("returnTo", returnTo);
            }    
        }

        public IActionResult OnPost(string name, string secret)
        {
            name = JsonUtils.RemoveAllWeirdness(name);
            // var isAdmin = secret == "Bergsklättring134";
            var isAdmin = !string.IsNullOrEmpty(secret);
            AnnaFest.User.SetUser(this.session, name, isAdmin);
            var returnTo = this.session.GetString("returnTo");
            if (!string.IsNullOrEmpty(returnTo))
            {
                return new RedirectResult("/" + returnTo);
            }
            else
            {
                return new RedirectResult("/Index");
            }
           
        }
    }
}
