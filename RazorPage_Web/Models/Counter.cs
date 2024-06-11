using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class Counter
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
		[Required]
		public string Location { get; set; }

		public ICollection<ApplicationUser> Users { get; set; }
	}
}
