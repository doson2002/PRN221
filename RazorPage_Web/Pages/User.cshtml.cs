using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages
{
    public class UserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        public ApplicationUser? appUser;
        private readonly AppDbContext _context;


        // Properties for counts
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalProducts { get; set; }

        public UserModel(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }
        public void OnGet()
        {
            // Retrieve data counts from database
            TotalCustomers = _context.Customers.Count();
            TotalOrders = _context.Orders.Count();
            TotalEmployees = _context.Users.Count();
            TotalProducts = _context.Products.Count();

            // Retrieve user data
            var task = userManager.GetUserAsync(User);
            task.Wait();
            appUser = task.Result;
        }
    }
}
