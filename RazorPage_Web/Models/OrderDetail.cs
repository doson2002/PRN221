using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		public int ProductID { get; set; }

        [Required]
        public int OrderID { get; set; } // Khóa ngoại tham chiếu đến Order

        [ForeignKey("OrderID")]
		public Order Order { get; set; }
	}
}
