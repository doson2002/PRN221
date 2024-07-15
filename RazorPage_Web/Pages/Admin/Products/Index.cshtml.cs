using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Products
{
    public class IndexModel : PageModel

    {
        private readonly AppDbContext context;

        public List<Product> Products { get; set; } = new List<Product>();

		[BindProperty(SupportsGet = true)]
		public string SearchBarcode { get; set; }

		// Pagination properties
		public int CurrentPage { get; set; } = 1;
		public int TotalPages { get; set; }
		public int PageSize { get; set; } = 3; // You can adjust the page size as needed

		public IndexModel(AppDbContext context)
        {
            this.context = context;
        }

		public async Task<IActionResult> OnGetAsync(int currentPage = 1)
		{
			CurrentPage = currentPage;
			var productsQuery = from p in context.Products
						   select p;

			if (!string.IsNullOrEmpty(SearchBarcode))
			{
				productsQuery = productsQuery.Where(p => p.Barcode.Contains(SearchBarcode));
			}
			int totalProducts = await productsQuery.CountAsync();
			TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

			Products = await productsQuery
		   .OrderByDescending(p => p.Id)
		   .Skip((CurrentPage - 1) * PageSize)
		   .Take(PageSize)
		   .ToListAsync();

			return Page();
		}
	}
}
