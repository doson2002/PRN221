using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage_Web.DAL;
using RazorPage_Web.Hubs;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Customers
{
    public class EditModel : PageModel
    {
		private readonly IWebHostEnvironment environment;
		private readonly AppDbContext _context;
		private readonly IHubContext<SignalRServer> _signalRHub;

		[BindProperty]
        public CustomerDTO customerDTO { get; set; } = new CustomerDTO();
        public Customer Customer { get; set; } = new Customer();

        public string errorMessage = "";
        public string successMessage = "";
		public EditModel(IWebHostEnvironment environment, AppDbContext context, IHubContext<SignalRServer> signalRHub)
        {
			this.environment = environment;
			this._context = context;
		    this._signalRHub = signalRHub;
        }

        
        public void OnGet(int ?id)
        {
            if (id == null)
            {
                Response.Redirect("/Admin/Customers/Index");
            }
            var customer = _context.Customers.Find(id);
            if(customer == null) 
            {
                Response.Redirect("/Admin/Customers/Index");
                return;
            }
            customerDTO.FullName = customer.FullName;
            customerDTO.Email = customer.Email;
            customerDTO.PhoneNumber = customer.PhoneNumber;
            customerDTO.Address = customer.Address;
            customerDTO.accumulated_point = customer.accumulated_point;
            Customer = customer;

        }
		public async Task<IActionResult> OnPostAsync(int? id)
		{
            if (id == null)
            {
                return RedirectToPage("Admin/Customers/Index");

            }
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
				return RedirectToPage("Admin/Customers/Index");
			}
            var customer = _context.Customers.Find(id);
            if( customer == null)
            {
				return RedirectToPage("Admin/Customers/Index");
			}

            //update Customer in the database
            customer.FullName = customerDTO.FullName;
            customer.Email = customerDTO.Email;
            customer.PhoneNumber = customerDTO.PhoneNumber;
            customer.Address = customerDTO.Address;
            customer.accumulated_point = customerDTO.accumulated_point;
			await _context.SaveChangesAsync();

			// Gửi thông báo SignalR
			await _signalRHub.Clients.All.SendAsync("ReceiveCustomerUpdate");

			Customer = customer;
            successMessage = "Customer updated successfully";
			return RedirectToPage("Admin/Customers/Index");
		}
    }
}
