using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Users
{
	public class IndexModel : PageModel
	{
		private readonly AppDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
		public Dictionary<string, string> UserRoles { get; set; } = new Dictionary<string, string>();

		public IndexModel(AppDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}


		public async Task OnGetAsync()
		{
			Users = await _context.Users.ToListAsync();

			foreach (var user in Users)
			{
				var userRoles = await _userManager.GetRolesAsync(user);
				var roleNames = string.Join(", ", userRoles);
				UserRoles[user.Id] = roleNames;
			}
		}
	}
}