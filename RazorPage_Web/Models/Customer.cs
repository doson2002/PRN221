using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	[Index(nameof(Email), IsUnique = true)]
	[Index(nameof(PhoneNumber), IsUnique = true)]
	public class Customer
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string FullName { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string Address { get; set; }
		[Required]
		public float accumulated_point { get; set; }

	}
}
