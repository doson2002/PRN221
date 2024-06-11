using Microsoft.AspNetCore.Identity;
using RazorPage_Web.Models;

namespace RazorPage_Web.DAL
{
	public class DbInitializer
	{
		public static async Task InitializeAsync(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			string[] roleNames = { "admin", "manager", "staff" };
			IdentityResult roleResult;

			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			var adminUser = new ApplicationUser
			{
				UserName = "admin@gmail.com",
				Email = "admin@gmail.com",
				EmailConfirmed = true
			};

			string adminPassword = "Son123456@";
			var user = await userManager.FindByEmailAsync("admin@gmail.com");

			if (user == null)
			{
				var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
				if (createAdmin.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "admin");
				}
			}
		}
	}
}