using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class CounterDTO
	{
		[Required]
		public string Name { get; set; } = "";
		[Required]
		public string Location { get; set; } = "";
	}

}
