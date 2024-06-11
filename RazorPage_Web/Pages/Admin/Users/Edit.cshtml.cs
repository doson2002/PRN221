using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly AppDbContext _context;

		[BindProperty]
		public ApplicationUser ApplicationUser { get; set; }
		[BindProperty]
		public string Role { get; set; }


		public List<Counter> Counters { get; set; }

		public string ErrorMessage { get; set; }

		public EditModel(UserManager<ApplicationUser> userManager, 
			RoleManager<IdentityRole> roleManager,
			AppDbContext context)
		{
			_userManager = userManager;
			_context = context;
			_roleManager = roleManager;
		}
		public async Task<IActionResult> OnGetAsync(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			ApplicationUser = await _userManager.FindByIdAsync(id);
			

			if (ApplicationUser == null)
			{
				return NotFound();
			}
			var userRoles = await _userManager.GetRolesAsync(ApplicationUser);
			Role = userRoles.FirstOrDefault();

			await LoadCountersAsync();
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var userToUpdate = await _userManager.FindByIdAsync(ApplicationUser.Id);

			if (userToUpdate == null)
			{
				return NotFound();
			}

			var userRoles = await _userManager.GetRolesAsync(userToUpdate);
			var currentRole = userRoles.FirstOrDefault();

			// Nếu không chọn vai trò mới, giữ lại vai trò hiện tại
			if (string.IsNullOrEmpty(Role))
			{
				Role = currentRole;
			}

			// Thực hiện thay đổi vai trò nếu cần
			if (currentRole != null && currentRole != Role)
			{
				await _userManager.RemoveFromRoleAsync(userToUpdate, currentRole);
				await _userManager.AddToRoleAsync(userToUpdate, Role);
					// Nếu thêm vai trò thành công, gán giá trị cho currentRole
				currentRole = Role;
				
			}
			else if (currentRole == null)
			{
				 await _userManager.AddToRoleAsync(userToUpdate, Role);
					currentRole= Role;
			
			}

				await LoadCountersAsync();
				userToUpdate.UserName = ApplicationUser.Email;
				userToUpdate.Email = ApplicationUser.Email;
				userToUpdate.FullName = ApplicationUser.FullName;
				userToUpdate.PhoneNumber = ApplicationUser.PhoneNumber;
				if(currentRole == "staff")
				{
					userToUpdate.CounterID = ApplicationUser.CounterID;
					// Nếu không phải là "manager" , "admin", kiểm tra ModelState.IsValid và gán CounterID và CounterLocation
					if (!ModelState.IsValid)
					{
						ErrorMessage = "Please provide all the required fields";

						// Gỡ lỗi ModelState
						foreach (var key in ModelState.Keys)
						{
							var state = ModelState[key];
							Console.WriteLine($"{key}: {state.ValidationState}");
						}
						return Page();
					}
				}
				else
				{
					userToUpdate.CounterID = null;
				}
	
			
			var result = await _userManager.UpdateAsync(userToUpdate);

			if (result.Succeeded)
			{
				TempData["SuccessMessage"] = "User updated successfully";
				return RedirectToPage("/Admin/Users/Index");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Page();
			}
		}
		private async Task LoadCountersAsync()
		{
			Counters = await _context.Counters.ToListAsync();
		}
		public async Task<IActionResult> OnGetToggleUserStatusAsync(string id)
		{
			ApplicationUser = await _userManager.FindByIdAsync(id);
			if (ApplicationUser == null)
			{
				return NotFound();
			}

			ApplicationUser.IsActive = !ApplicationUser.IsActive;
			var result = await _userManager.UpdateAsync(ApplicationUser);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Page(); // Trở lại trang hiện tại
			}

			return RedirectToPage("/Admin/Users/Index");
		}
	}
}