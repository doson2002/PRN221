using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Counters
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        private readonly AppDbContext _context;
        [BindProperty]
        public CounterDTO counterDTO { get; set; } = new CounterDTO();
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
			bool locationExists = _context.Counters.Any(c => c.Location == counterDTO.Location);
			if (locationExists)
			{
				ModelState.AddModelError("counterDTO.Location", "The counter Location already exists.");
				errorMessage = "The counter Location already exists. Please use a different counter Location.";
				return;
			}
		

			//save the new product in database
			Counter counter = new Counter()
            {
                Name = counterDTO.Name,
                Location = counterDTO.Location,
            };
            _context.Counters.Add(counter);
            _context.SaveChanges();


			//clear the form
			counterDTO.Name = "";
			counterDTO.Location = "";
		

            ModelState.Clear();

            successMessage = "Counter created successfully";

            Response.Redirect("/Admin/Counters/Index");
        }
    }
}
