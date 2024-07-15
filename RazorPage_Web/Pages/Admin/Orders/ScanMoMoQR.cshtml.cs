using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.Services;

namespace RazorPage_Web.Pages.Admin.Orders
{
    public class ScanMoMoQRModel : PageModel
    {
        private readonly MoMoPaymentService _moMoPaymentService;
        private readonly IConfiguration _configuration;

        public ScanMoMoQRModel(MoMoPaymentService moMoPaymentService, IConfiguration configuration)
        {
            _moMoPaymentService = moMoPaymentService;
            _configuration = configuration;
        }


		public async Task<IActionResult> OnGetAsync(string orderId, decimal total, string orderInfo)
		{
			 orderInfo = "Thanh toán đơn hàng ABC";
			if (string.IsNullOrEmpty(orderId))
			{
				return BadRequest("orderId không được trống");
			}

			if (string.IsNullOrEmpty(orderInfo))
			{
				return BadRequest("orderInfo không được trống");
			}

            try
            {
                // Gọi MoMoPaymentService để tạo yêu cầu thanh toán
                string payUrl = await _moMoPaymentService.CreatePaymentRequest(orderId, (long)total, orderInfo);

                // Log the payUrl for debugging
                Console.WriteLine("Pay URL: " + payUrl);

                // Redirect đến payUrl để thanh toán
                return Redirect(payUrl);
            }
            catch (Exception ex)
			{
				// Xử lý lỗi tại đây
				Console.WriteLine("Lỗi: " + ex.Message);
				return BadRequest("Đã xảy ra lỗi khi tạo yêu cầu thanh toán");
			}
		}
	}
}
