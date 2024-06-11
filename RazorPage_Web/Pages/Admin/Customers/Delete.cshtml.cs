using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;

namespace RazorPage_Web.Pages.Admin.Customers
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
                Response.Redirect("/Admin/Customers/Index");
                return;
            }
            var customer = context.Customers.Find(id);
            if (customer == null)
            {
				Response.Redirect("/Admin/Customers/Index");
				return;
			}
            context.Customers.Remove(customer);
            context.SaveChanges();

			Response.Redirect("/Admin/Customers/Index");
		}
    }
}
