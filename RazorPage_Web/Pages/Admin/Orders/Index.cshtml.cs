using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Orders

{
    public class IndexModel : PageModel
    {
		private readonly AppDbContext _context;
		public List<Order> Orders { get; set; } = new List<Order>();

        public IndexModel(AppDbContext context)
        {
            this._context = context;
        }

        public void OnGet()
        {
			Orders = _context.Orders.OrderByDescending(o => o.Id).ToList();
		}
    }
}
