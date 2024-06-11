using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Counters

{
    public class IndexModel : PageModel
    {
		private readonly AppDbContext _context;
		public List<Counter> Counters { get; set; } = new List<Counter>();

        public IndexModel(AppDbContext context)
        {
            this._context = context;
        }

        public void OnGet()
        {
			Counters = _context.Counters.OrderByDescending(c => c.Id).ToList();
		}
    }
}
