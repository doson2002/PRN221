using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;


namespace RazorPage_Web.Pages.Admin.CustomerPolicies
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public CustomerPolicy CustomerPolicy { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6; // You can adjust the page size as needed
        public List<CustomerPolicy> CustomerPolicies { get; set; } = new List<CustomerPolicy>();

        public IndexModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;

        }
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

        public string errorMessage = "";
        public string successMessage = "";
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                errorMessage = "User is not logged in.";
                return Page();
            }
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            CustomerPolicy = JsonConvert.DeserializeObject<CustomerPolicy>(json);

            var policy = new CustomerPolicy
            {
                Description = CustomerPolicy.Description,
                DiscountRate = CustomerPolicy.DiscountRate,
                FixedDiscountAmount = CustomerPolicy.FixedDiscountAmount,
                CreatedBy = user.Email,
                ApprovalDate = DateTime.Now,
                ValidFrom = CustomerPolicy.ValidFrom,
                ValidTo = CustomerPolicy.ValidTo,
                PublishingStatus = "Waiting",
            };

            _context.CustomerPolicies.Add(policy);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });

        }
        public async Task<IActionResult> OnGetUpdateStatusAsync(int id, string status)
        {
            var policy = await _context.CustomerPolicies.FindAsync(id);
            if (policy == null)
            {
                return NotFound();
            }

            policy.PublishingStatus = status;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
