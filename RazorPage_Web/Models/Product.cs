using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace RazorPage_Web.Models
{
	[Index(nameof(Barcode), IsUnique = true)]
	public class Product
    {
        [Key]   
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [Required]
		public string Barcode { get; set; }

        [Required]
        public double PriceProcessing { get; set; }
        [Required]
        public double PriceStone { get; set; }
        [Required]
        public double PriceRate { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Weight { get; set; }
        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";
        
        [Required]
        public int TypePriceId { get; set; }




    }
}
