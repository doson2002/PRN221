using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class CustomerPoliciesDTO
	{

		public string Description;

		public double DiscountRate;

		public double FixedDiscountAmount;

		[Required] public DateTime ValidFrom;
		[Required] public DateTime ValidTo;

		[Required] public string PublishingStatus;
	}
}
