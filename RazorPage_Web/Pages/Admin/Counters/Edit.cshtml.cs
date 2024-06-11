using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Counters
{
    public class EditModel : PageModel
    {
		private readonly IWebHostEnvironment environment;
		private readonly AppDbContext _context;

        [BindProperty]
        public CounterDTO counterDTO { get; set; } = new CounterDTO();
        public Counter Counter { get; set; } = new Counter();

        public string errorMessage = "";
        public string successMessage = "";
		public EditModel(IWebHostEnvironment environment, AppDbContext context)
        {
			this.environment = environment;
			this._context = context;
		}

        
        public void OnGet(int ?id)
        {
            if (id == null)
            {
                Response.Redirect("/Admin/Counters/Index");
            }
            var counter = _context.Counters.Find(id);
            if(counter == null) 
            {
                Response.Redirect("/Admin/Counters/Index");
                return;
            }
            counterDTO.Name = counter.Name;
            counterDTO.Location = counter.Location;
         
            Counter = counter;

        }
       public void OnPost(int? id)
        {
            if (id == null)
            {
                Response.Redirect("Admin/Counters/Index");
                return;

            }
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
                return;
			}
            var counter = _context.Counters.Find(id);
            if(counter == null)
            {
                Response.Redirect("/Admin/Counters/Index");
                return;
            }

			//update Customer in the database
			counter.Name = counterDTO.Name;
            counter.Location = counterDTO.Location;
            _context.SaveChanges();

            Counter = counter;
            successMessage = "Counter updated successfully";
            Response.Redirect("/Admin/Counters/Index");
        }
    }
}
