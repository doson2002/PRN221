using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class OrderDetail
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int Quantity { get; set; } = 0;
		[Required]
		public double UnitPrice { get; set; } = 0;

		[Required]
		public int OrderID { get; set; }
		[Required]
		public int ProductID { get; set; }
	}
}
