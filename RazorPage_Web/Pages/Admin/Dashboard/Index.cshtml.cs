using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.Models;
using RazorPage_Web.Services;

namespace RazorPage_Web.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly OrderService _orderService;

        public double CurrentYearRevenue { get; set; }
        public IndexModel(OrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            CurrentYearRevenue = await _orderService.GetCurrentYearRevenueAsync();
            return Page();
        }
    }
}
