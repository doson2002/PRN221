using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;

namespace RazorPage_Web.Pages.Admin.Products
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
                Response.Redirect("/Admin/Products/Index");
                return;
            }
            var product = context.Products.Find(id);
            if (product == null)
            {
				Response.Redirect("/Admin/Products/Index");
				return;
			}
            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);
            context.Products.Remove(product);
            context.SaveChanges();

			Response.Redirect("/Admin/Products/Index");
		}
    }
}
