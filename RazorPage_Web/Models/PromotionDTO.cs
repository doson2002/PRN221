using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
    public class PromotionDTO
    {
        [Required]
        public string Code { get; set; } = "";
        [Required]
        public double DiscountPercentage { get; set; }
        [Required]
        public int FixedDiscountAmount { get; set; }
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
}
