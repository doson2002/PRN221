using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Staff.Products
{
    public class IndexModel : PageModel

    {
        private readonly AppDbContext context;

        public List<Product> Products { get; set; } = new List<Product>();

        public IndexModel(AppDbContext context)
        {
            this.context = context;
        }

        public void OnGet()
        {
            Products = context.Products.OrderByDescending(p => p.Id).ToList();
        }
    }
}
