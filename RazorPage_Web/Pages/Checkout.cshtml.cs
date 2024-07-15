using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.Services;

namespace RazorPage_Web.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly MoMoPaymentService _moMoPaymentService;

        public CheckoutModel(MoMoPaymentService moMoPaymentService)
        {
            _moMoPaymentService = moMoPaymentService;
        }

        [BindProperty]
        public long Amount { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string orderId = "Order" + DateTime.Now.Ticks;
            string orderInfo = "Payment for order " + orderId;
            string payUrl = await _moMoPaymentService.CreatePaymentRequest(orderId, Amount, orderInfo);

            return Redirect(payUrl);
        }
    }
}
