using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Staff.Promotions

{
    public class IndexModel : PageModel
    {
		private readonly AppDbContext _context;
		public List<Promotion> Promotions { get; set; } = new List<Promotion>();

        public IndexModel(AppDbContext context)
        {
            this._context = context;
        }

        public void OnGet()
        {
			Promotions = _context.Promotions.OrderByDescending(p => p.Id).ToList();
		}
    }
}
