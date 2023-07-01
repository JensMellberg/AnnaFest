using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    public class AdminModel : BasePage
    {

        public AdminModel(IWebHostEnvironment environment, IHttpContextAccessor session) : base(environment, session)
        {
        }

        public IActionResult OnGet()
        {
            if (!this.User.IsAdmin)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        /*public IActionResult OnPost(string action)
        {
            
        }*/
    }
}
