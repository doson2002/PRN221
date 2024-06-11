using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class CustomerDTO
	{
		public string FullName { get; set; } = "";
		[Required]
		public string Email { get; set; } = "";
		[Required]
		public string PhoneNumber { get; set; } = "";
		[Required] 
		public string Address { get; set; } = "";
		[Required]
		public float accumulated_point { get; set; }
	}
}
