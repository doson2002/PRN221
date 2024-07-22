using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Staff.Orders

{
	public class IndexModel : PageModel
	{
		private readonly AppDbContext _context;

		[BindProperty(SupportsGet = true)]
		public DateTime? StartDate { get; set; }

		[BindProperty(SupportsGet = true)]
		public DateTime? EndDate { get; set; }


		public string SuccessMessage { get; set; } // Thuộc tính để lưu successMessage

		public List<Order> Orders { get; set; } = new List<Order>();

		public IndexModel(AppDbContext context)
		{
			this._context = context;
		}

		public void OnGet()
		{
			TempData.Clear();
			SuccessMessage = HttpContext.Session.GetString("SuccessMessage");
			HttpContext.Session.Remove("SuccessMessage");

			IQueryable<Order> query = _context.Orders.OrderByDescending(o => o.Id);

			if (StartDate.HasValue)
			{
				query = query.Where(o => o.Date >= StartDate.Value);
			}

			if (EndDate.HasValue)
			{
				query = query.Where(o => o.Date <= EndDate.Value);
			}

			Orders = query.ToList();
		}

	}
}
