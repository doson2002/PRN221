using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Staff.Customers
{
    public class IndexModel : PageModel
    {
		private readonly AppDbContext _context;
		public List<Customer> Customers { get; set; } = new List<Customer>();

        public IndexModel(AppDbContext context)
        {
            this._context = context;
        }

        public void OnGet()
        {
			Customers = _context.Customers.OrderByDescending(c => c.Id).ToList();
		}
    }
}
