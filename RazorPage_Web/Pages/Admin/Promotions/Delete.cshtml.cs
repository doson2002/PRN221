using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;

namespace RazorPage_Web.Pages.Admin.Promotions
{
    public class DeleteModel : PageModel
    {
		private readonly IWebHostEnvironment environment;
		private readonly AppDbContext context;

		public DeleteModel(IWebHostEnvironment environment, AppDbContext context)
        {
			this.environment = environment;
			this.context = context;
		}
        public void OnGet(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/Admin/Promotions/Index");
                return;
            }
            var promotion = context.Promotions.Find(id);
            if (promotion == null)
            {
				Response.Redirect("/Admin/Promotions/Index");
				return;
			}
            context.Promotions.Remove(promotion);
            context.SaveChanges();

			Response.Redirect("/Admin/Promotions/Index");
		}
    }
}
