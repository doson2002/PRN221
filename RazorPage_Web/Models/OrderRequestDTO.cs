using Microsoft.Build.Framework;

namespace RazorPage_Web.Models
{
	public class OrderRequestDTO
	{
		[Required]
		public int ProductId { get; set; }

		[Required]
		public int Quantity { get; set; }
		[Required]
		public double UnitPrice { get; set; }
	}
}
