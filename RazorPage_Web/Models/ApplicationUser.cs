using Microsoft.AspNetCore.Identity;

namespace RazorPage_Web.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; } = "";

		public string Address { get; set; } = "";

		public bool IsActive { get; set; } = true;

		public int? CounterID { get; set; }

	}
}
