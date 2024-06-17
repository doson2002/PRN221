using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Promotions
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        private readonly AppDbContext _context;

        [BindProperty]
        public PromotionDTO promotionDTO { get; set; } = new PromotionDTO();
        public CreateModel(IWebHostEnvironment environment, AppDbContext context)
        {
            this.environment = environment;
            this._context  = context;

        }
        public void OnGet()
        {
        }
        public string errorMessage = "";
        public string successMessage = "";
        public void OnPost() 
        { 
            
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
                return;
            }
			// Kiểm tra trùng lặp Email
			bool codeExists = _context.Promotions.Any(c => c.Code == promotionDTO.Code);
			if (codeExists)
			{
				ModelState.AddModelError("promotionDTO.Code", "The code already exists.");
				errorMessage = "The code already exists. Please use a different code.";
				return;
			}
		

			//save the new product in database
			Promotion promotion = new Promotion()
            {
                Code = promotionDTO.Code,
                DiscountPercentage = promotionDTO.DiscountPercentage,
                FixedDiscountAmount = promotionDTO.FixedDiscountAmount,
                StartDate = promotionDTO.StartDate,
                EndDate = promotionDTO.EndDate
            };
            _context.Promotions.Add(promotion);
            _context.SaveChanges();


            //clear the form
            promotionDTO.Code = "";
            promotionDTO.FixedDiscountAmount = 0;
            promotionDTO.DiscountPercentage = 0;
            promotionDTO.StartDate = DateTime.Now;
            promotionDTO.EndDate = DateTime.Now;



			ModelState.Clear();

            successMessage = "Promotion created successfully";

            Response.Redirect("/Admin/Promotions/Index");
        }
    }
}
