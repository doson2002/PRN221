using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Promotions
{
    public class PromotionService
    {
        private readonly AppDbContext _context;

        public PromotionService(AppDbContext context)
        {
            _context = context;
        }

        public List<Promotion> GetAllPromotions()
        {
            return _context.Promotions.ToList();
        }

        public Promotion GetPromotionByCode(string code)
        {
            return _context.Promotions.FirstOrDefault(p => p.Code == code);
        }

        public bool IsPromotionValid(string code)
        {
            var promotion = GetPromotionByCode(code);
            if (promotion != null && promotion.StartDate <= DateTime.Now && promotion.EndDate >= DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public double CalculateDiscountAmount(string code, double originalPrice)
        {
            var promotion = GetPromotionByCode(code);
            if (promotion != null)
            {
                if (promotion.DiscountPercentage > 0)
                {
                    // Áp dụng giảm giá theo phần trăm
                    return originalPrice * promotion.DiscountPercentage / 100.0;
                }
                else if (promotion.FixedDiscountAmount > 0)
                {
                    // Áp dụng giảm giá cố định
                    return promotion.FixedDiscountAmount;
                }
            }
            return 0; // Không có giảm giá áp dụng
        }
    }
}
