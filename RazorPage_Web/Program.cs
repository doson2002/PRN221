using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using Microsoft.AspNetCore.Identity;
using RazorPage_Web.Models;
using RazorPage_Web.Pages.Admin.Promotions;
using RazorPage_Web.Hubs;
using RazorPage_Web.Services;

namespace RazorPage_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
  
            // Add services to the container.
            builder.Services.AddRazorPages();
            // Add session services
            builder.Services.AddSession();
            builder.Services.AddDistributedMemoryCache(); // Needed for session state

			builder.Services.AddSignalR();
			//DI
			builder.Services.AddDbContext<AppDbContext>
                (options =>
                options.UseSqlServer
                (builder.Configuration.GetConnectionString
                ("DefaultConnection"))
                );

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            // Add PromotionService
            builder.Services.AddScoped<PromotionService>();
            builder.Services.AddScoped<OrderService>();

			builder.Services.AddHttpClient();
			builder.Services.AddTransient<MoMoPaymentService>();


			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication(); // Add this line to enable authentication

            app.UseAuthorization();
           


            app.MapRazorPages();

			app.UseEndpoints(endpoints =>
			{
                endpoints.MapRazorPages();

				endpoints.MapHub<SignalRServer>("/signalrServer");
			});

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
				DbInitializer.InitializeAsync(services, userManager).Wait();
			}

			app.Run();
        }
    }
}