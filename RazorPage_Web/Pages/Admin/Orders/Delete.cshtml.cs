using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;

namespace RazorPage_Web.Pages.Admin.Orders
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
                Response.Redirect("/Admin/Orders/Index");
                return;
            }
            var order = context.Orders.Find(id);
            if (order == null)
            {
				Response.Redirect("/Admin/Orders/Index");
				return;
			}
            context.Orders.Remove(order);
            context.SaveChanges();

			Response.Redirect("/Admin/Orders/Index");
		}
    }
}
