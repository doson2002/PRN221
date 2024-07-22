using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using RazorPage_Web.Hubs;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Customers
{
    public class IndexModel : PageModel
    {
		private readonly AppDbContext _context;
		private readonly IHubContext<SignalRServer> _signalRHub;
		public List<Customer> Customers { get; set; } = new List<Customer>();


		[BindProperty(SupportsGet = true)]
		public string SearchTerm { get; set; }

		// Pagination properties
		public int CurrentPage { get; set; } = 1;
		public int TotalPages { get; set; }
		public int PageSize { get; set; } = 3; // You can adjust the page size as needed

		public IndexModel(AppDbContext context, IHubContext<SignalRServer> signalRHub)
        {
            this._context = context;
			this._signalRHub = signalRHub;
        }

        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
			CurrentPage = currentPage;
			var customersQuery = from c in _context.Customers
						select c;
			if (!string.IsNullOrEmpty(SearchTerm))
			{
				customersQuery = customersQuery.Where(c => c.FullName.Contains(SearchTerm) || c.PhoneNumber.Contains(SearchTerm));
			}
			int totalCustomers = await customersQuery.CountAsync();
			TotalPages = (int)Math.Ceiling(totalCustomers / (double)PageSize);

			await _signalRHub.Clients.All.SendAsync("ReceiveCustomerUpdate", customersQuery);
			Customers = await customersQuery
					   .OrderByDescending(c => c.Id)
					   .Skip((CurrentPage - 1) * PageSize)
					   .Take(PageSize)
					   .ToListAsync(); 
			return Page();
		}
	}
}
