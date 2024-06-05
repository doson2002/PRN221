using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_Web.Pages
{
    [Authorize(Roles = "manager")]
    public class ManagerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
