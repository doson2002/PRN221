using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Staff.Customers
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        private readonly AppDbContext _context;
        [BindProperty]
        public CustomerDTO customerDTO { get; set; } = new CustomerDTO();
        public CreateModel(IWebHostEnvironment environment, AppDbContext context)
        {
            this.environment = environment;
            this._context  = context;

        }
        public void OnGet()
        {
        }
        public string errorMessage = "";
        public string successMessage = "";
        public void OnPost() 
        { 
            
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
                return;
            }
			// Kiểm tra trùng lặp Email
			bool emailExists = _context.Customers.Any(c => c.Email == customerDTO.Email);
			if (emailExists)
			{
				ModelState.AddModelError("customerDTO.Email", "The email already exists.");
				errorMessage = "The email already exists. Please use a different email.";
				return;
			}
			// Kiểm tra trùng lặp Phone
			bool phoneExists = _context.Customers.Any(c => c.PhoneNumber == customerDTO.PhoneNumber);
			if (phoneExists)
			{
				ModelState.AddModelError("customerDTO.PhoneNumber", "The Phone Number already exists.");
				errorMessage = "The Phone Number already exists. Please use a different Phone Number.";
				return;
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
            _context.SaveChanges();


			//clear the form
			customerDTO.FullName = "";
			customerDTO.Email = "";
			customerDTO.PhoneNumber = "";
			customerDTO.Address = "";
            customerDTO.accumulated_point = 0;

            ModelState.Clear();

            successMessage = "Customer created successfully";

            Response.Redirect("/Staff/Customers/Index");
        }
    }
}
