using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;



namespace RazorPage_Web.Pages.Staff.CustomerPolicies
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public CustomerPolicy CustomerPolicy { get; set; }

        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }
        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6; // You can adjust the page size as needed


        public List<CustomerPolicy> CustomerPolicies { get; set; }


        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;
            int totalPolicies = await _context.CustomerPolicies.CountAsync();
            TotalPages = (int)Math.Ceiling(totalPolicies / (double)PageSize);

            CustomerPolicies = await _context.CustomerPolicies
           .OrderByDescending(p => p.Id)
           .Skip((CurrentPage - 1) * PageSize)
           .Take(PageSize)
           .ToListAsync();

            return Page();
        }

       

    }
}
