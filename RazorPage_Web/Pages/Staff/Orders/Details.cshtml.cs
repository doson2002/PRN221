using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.Models;
using RazorPage_Web.Services;

namespace RazorPage_Web.Pages.User.Orders
{
    public class DetailsModel : PageModel
    {
		private readonly OrderService _orderService;

		public Order Order { get; set; }

		public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
		public DetailsModel(OrderService orderService)
		{
			_orderService = orderService;
		}
		public async Task<IActionResult> OnGetAsync(int id)
        {
			Order = await _orderService.GetOrderWithDetailsAsync(id);
			if (Order == null)
			{
				return NotFound();
			}
			OrderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(id);
			return Page();
        }
    }
}
