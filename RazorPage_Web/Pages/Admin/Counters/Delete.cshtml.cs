using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;

namespace RazorPage_Web.Pages.Admin.Counters
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
                Response.Redirect("/Admin/Counters/Index");
                return;
            }
            var counter = context.Counters.Find(id);
            if (counter == null)
            {
				Response.Redirect("/Admin/Counters/Index");
				return;
			}
            context.Counters.Remove(counter);
            context.SaveChanges();

			Response.Redirect("/Admin/Counters/Index");
		}
    }
}
