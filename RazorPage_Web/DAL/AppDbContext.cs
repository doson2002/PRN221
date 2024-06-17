using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.Models;

namespace RazorPage_Web.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<Customer> Customers { get; set; }
		public virtual DbSet<Counter> Counters { get; set; }

		public virtual DbSet<Promotion> Promotions { get; set; }

		public virtual DbSet<Order> Orders { get; set; }

		public virtual DbSet<OrderDetail> OrderDetails { get; set; }

		public virtual DbSet<TypePrice> TypePrices { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

            var admin = new IdentityRole("admin");
            admin.NormalizedName = "admin";

			var manager = new IdentityRole("manager");
			manager.NormalizedName = "manager";

			var staff = new IdentityRole("staff");
			staff.NormalizedName = "staff";

			builder.Entity<IdentityRole>().HasData(admin, manager, staff);

		
		}
	}

}
