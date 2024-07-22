using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage_Web.DAL;
using RazorPage_Web.Hubs;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Customers
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        private readonly AppDbContext _context;
		private readonly IHubContext<SignalRServer> _signalRHub;
		[BindProperty]
        public CustomerDTO customerDTO { get; set; } = new CustomerDTO();
        public CreateModel(IWebHostEnvironment environment, AppDbContext context, IHubContext<SignalRServer> signalRHub)
        {
            this.environment = environment;
            this._context  = context;
            this._signalRHub = signalRHub;

        }
        public void OnGet()
        {
        }
        public string errorMessage = "";
        public string successMessage = "";
		public async Task<IActionResult> OnPostAsync()
		{ 
            
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
				return Page();
			}
			// Kiểm tra trùng lặp Email
			bool emailExists = _context.Customers.Any(c => c.Email == customerDTO.Email);
			if (emailExists)
			{
				ModelState.AddModelError("customerDTO.Email", "The email already exists.");
				errorMessage = "The email already exists. Please use a different email.";
				return Page();
			}
			// Kiểm tra trùng lặp Phone
			bool phoneExists = _context.Customers.Any(c => c.PhoneNumber == customerDTO.PhoneNumber);
			if (phoneExists)
			{
				ModelState.AddModelError("customerDTO.PhoneNumber", "The Phone Number already exists.");
				errorMessage = "The Phone Number already exists. Please use a different Phone Number.";
				return Page();
			}

			//save the new product in database
			Customer customer = new Customer()
            {
                FullName = customerDTO.FullName,
                Email = customerDTO.Email,
                PhoneNumber = customerDTO.PhoneNumber,
                Address = customerDTO.Address,
                accumulated_point = customerDTO.accumulated_point
            };
            _context.Customers.Add(customer);
			await _context.SaveChangesAsync();

			// Gửi thông báo SignalR
			await _signalRHub.Clients.All.SendAsync("ReceiveCustomerUpdate");


			//clear the form
			customerDTO.FullName = "";
			customerDTO.Email = "";
			customerDTO.PhoneNumber = "";
			customerDTO.Address = "";
            customerDTO.accumulated_point = 0;

            ModelState.Clear();

            successMessage = "Customer created successfully";

			return RedirectToPage("/Admin/Customers/Index");
        }
    }
}
