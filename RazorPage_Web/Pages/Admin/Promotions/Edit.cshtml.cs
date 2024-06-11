using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Promotions
{
    public class EditModel : PageModel
    {
		private readonly IWebHostEnvironment environment;
		private readonly AppDbContext _context;

        [BindProperty]
        public PromotionDTO promotionDTO { get; set; } = new PromotionDTO();
        public Promotion Promotion { get; set; } = new Promotion();

        public string errorMessage = "";
        public string successMessage = "";
		public EditModel(IWebHostEnvironment environment, AppDbContext context)
        {
			this.environment = environment;
			this._context = context;
		}

        
        public void OnGet(int ?id)
        {
            if (id == null)
            {
                Response.Redirect("/Admin/Promotions/Index");
            }
            var promotion = _context.Promotions.Find(id);
            if(promotion == null) 
            {
                Response.Redirect("/Admin/Promotions/Index");
                return;
            }
            promotionDTO.Code = promotion.Code;
            promotionDTO.DiscountPercentage = promotion.DiscountPercentage;
            promotionDTO.FixedDiscountAmount = promotion.FixedDiscountAmount;
            promotionDTO.StartDate = promotion.StartDate;
            promotionDTO.EndDate = promotion.EndDate;
         
            Promotion = promotion;

        }
       public void OnPost(int? id)
        {
            if (id == null)
            {
                Response.Redirect("Admin/Promotions/Index");
                return;

            }
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
                return;
			}
            var promotion = _context.Promotions.Find(id);
            if(promotion == null)
            {
                Response.Redirect("/Admin/Promotions/Index");
                return;
            }

			//update Customer in the database
			promotion.Code = promotionDTO.Code;
            promotion.DiscountPercentage = promotionDTO.DiscountPercentage;
            promotion.FixedDiscountAmount = promotionDTO.FixedDiscountAmount;
            promotion.StartDate = promotionDTO.StartDate;
            promotion.EndDate = promotionDTO.EndDate;
            _context.SaveChanges();

            Promotion = promotion;
            successMessage = "Promotion updated successfully";
            Response.Redirect("/Admin/Promotions/Index");
        }
    }
}
