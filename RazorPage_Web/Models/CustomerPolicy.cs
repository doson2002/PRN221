using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class CustomerPolicy
	{
		[Key]
        public int Id { get; set; }

        public string? Description { get; set; }

		public double? DiscountRate { get; set; }

		public double? FixedDiscountAmount { get; set; }

		[Required]
		public DateTime ValidFrom { get; set; } = DateTime.Now;
		[Required]
		public DateTime ValidTo { get; set; } = DateTime.Now;

		[Required] public string CreatedBy { get; set; }

		public string? ApprovedBy { get; set; }

		public DateTime? ApprovalDate { get; set; }

		[Required] public string PublishingStatus { get; set; }

	}
}
