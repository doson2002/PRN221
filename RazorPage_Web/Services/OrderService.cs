using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Services
{
	public class OrderService
	{
		private readonly AppDbContext _context;
		private readonly List<Order> _orders; // Giả sử đây là danh sách đơn hàng từ cơ sở dữ liệu

		public OrderService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Order> GetOrderWithDetailsAsync(int orderId)
		{
			return await _context.Orders
						   .Include(o => o.OrderDetails)
						   .FirstOrDefaultAsync(o => o.Id == orderId);
		}

		public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
		{
			return  await _context.OrderDetails
						   .Where(od => od.OrderID == orderId)
						   .ToListAsync();
		}

		public Task<List<Order>> GetOrdersAsync(DateTime? startDate, DateTime? endDate)
		{
			var query = _orders.AsQueryable();

			if (startDate.HasValue)
			{
				query = query.Where(o => o.Date >= startDate.Value);
			}

			if (endDate.HasValue)
			{
				query = query.Where(o => o.Date <= endDate.Value);
			}

			return Task.FromResult(query.ToList());
		}
        public async Task<double> GetCurrentYearRevenueAsync()
        {
            int currentYear = DateTime.Now.Year;
            var totalRevenue = await _context.OrderDetails
                .Where(od => od.Order.Date.Year == currentYear)
                .SumAsync(od => od.Quantity * od.UnitPrice - od.Order.Discount);

            return totalRevenue;
        }
    }
}
