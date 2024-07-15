using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.Models;
using System.Reflection.Emit;

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
        public virtual DbSet<CustomerPolicy> CustomerPolicies { get; set; }




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

			builder.Entity<OrderDetail>()
		.HasOne<Order>(od => od.Order) // Trỏ đến thuộc tính navigation 'Order' trong 'OrderDetail'
         .WithMany(o => o.OrderDetails) // Order có thể có nhiều OrderDetail
		 .HasForeignKey(od => od.OrderID) // Khóa ngoại là OrderID
		 .OnDelete(DeleteBehavior.Cascade); // Cấu hình để tự động xóa các OrderDetail khi Order bị xóa
		}

	}
}
