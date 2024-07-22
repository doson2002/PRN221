using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage_Web.DAL;
using RazorPage_Web.Hubs;

namespace RazorPage_Web.Pages.Admin.Customers
{
    public class DeleteModel : PageModel
    {
		private readonly IWebHostEnvironment environment;
		private readonly AppDbContext context;
		private readonly IHubContext<SignalRServer> _signalRHub;

		public DeleteModel(IWebHostEnvironment environment, AppDbContext context, IHubContext<SignalRServer> signalRHub)
        {
			this.environment = environment;
			this.context = context;
			this._signalRHub = signalRHub;
		}
		public async Task<IActionResult> OnGetAsync(int? id)
		{
            if (id == null)
            {
				return RedirectToPage("Admin/Customers/Index");

			}
			var customer = context.Customers.Find(id);
            if (customer == null)
            {
				return RedirectToPage("Admin/Customers/Index");

			}
			context.Customers.Remove(customer);
			await context.SaveChangesAsync();

			// Gửi thông báo SignalR
			await _signalRHub.Clients.All.SendAsync("ReceiveCustomerUpdate");

			return RedirectToPage("Admin/Customers/Index");

		}
	}
}
