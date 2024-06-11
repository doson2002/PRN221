using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;
using System.Threading.Tasks;

namespace RazorPage_Web.Pages.Admin.Users
{
	public class DeleteModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public DeleteModel(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IActionResult> OnGetAsync(string id)
		{
			if (id == null)
			{
				return RedirectToPage("/Admin/Users/Index");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return RedirectToPage("/Admin/Users/Index");
			}

			// Thực hiện xóa người dùng
			var result = await _userManager.DeleteAsync(user);
			if (result.Succeeded)
			{
				return RedirectToPage("/Admin/Users/Index");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return Page();
			}
		}
	}
}
