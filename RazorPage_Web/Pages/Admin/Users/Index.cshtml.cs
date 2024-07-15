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
		

		[BindProperty(SupportsGet = true)]
		public string SearchTerm { get; set; }

		// Pagination properties
		public int CurrentPage { get; set; } = 1;
		public int TotalPages { get; set; }
		public int PageSize { get; set; } = 3; // You can adjust the page size as needed


		public IndexModel(AppDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}


		public async Task OnGetAsync(int currentPage = 1)
		{
			CurrentPage = currentPage;
			var usersQuery = from u in _context.Users
						select u;
			if (!string.IsNullOrEmpty(SearchTerm))
			{
				usersQuery = usersQuery.Where(u => u.UserName.Contains(SearchTerm) || u.PhoneNumber.Contains(SearchTerm));
			}
			int totalUsers = await usersQuery.CountAsync();
			TotalPages = (int)Math.Ceiling(totalUsers / (double)PageSize);
			Users = await usersQuery
					   .OrderByDescending(c => c.Id)
					   .Skip((CurrentPage - 1) * PageSize)
					   .Take(PageSize)
					   .ToListAsync();

			foreach (var user in Users)
			{
				var userRoles = await _userManager.GetRolesAsync(user);
				var roleNames = string.Join(", ", userRoles);
				UserRoles[user.Id] = roleNames;
			}
		}
	}
}