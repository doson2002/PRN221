using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class TypePrice
	{
		[Key] public int Id { get; set; }
		[Required] public DateTime Date { get; set; }
		[Required] public double PurchasePricePerGram { get; set; }
		[Required] public double SellPricePerGram { get; set; }
		[Required] public string Type { get; set; }
	}
}
