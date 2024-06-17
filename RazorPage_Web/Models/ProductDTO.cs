using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	
	public class ProductDTO
	{
		[Required, MaxLength(100)]
		public string ProductName { get; set; }
		[Required, MaxLength(100)]
		public string Barcode { get; set; }
		[Required]
		public string Description { get; set; }
        [Required]
        public double PriceProcessing { get; set; }
        [Required]
        public double PriceStone { get; set; }
        [Required]
        public double PriceRate { get; set; }
        [Required]
		public int Quantity { get; set; }
		[Required]
		public double Weight { get; set; }
		public IFormFile? ImageFile { get; set; }

        [Required]
        public int TypePriceId { get; set; }
    }
}
