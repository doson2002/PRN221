using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class Order
	{
		[Key]
		public int Id { get; set; }
		[Required] public DateTime Date { get; set; } = DateTime.Now;
		[Required]public double Discount { get; set; }
		 public string CreatedBy { get; set; }
		[Required] public string Type { get; set; }
		[Required] public int CustomerID { get; set; }
		public ICollection<OrderDetail> OrderDetails { get; set; }

	}
}
