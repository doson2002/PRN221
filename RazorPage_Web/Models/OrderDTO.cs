using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class OrderDTO
	{
		[Required] public DateTime Date { get; set; } = DateTime.Now;
		[Required]public double Discount { get; set; }
		 public string CreatedBy { get; set; }
		[Required] public string Type { get; set; }
		[Required] public int CustomerID { get; set; }
	}
}
