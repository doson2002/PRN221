using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;
using RazorPage_Web.Pages.Admin.Promotions;
using System;

namespace RazorPage_Web.Pages.Admin.Orders
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        private readonly PromotionService _promotionService;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public OrderDTO orderDTO { get; set; } = new OrderDTO();
		[BindProperty(SupportsGet = true)]
		public string Type { get; set; }


        [BindProperty]
        public int CustomerId { get; set; }

        [BindProperty]
        public int PromotionId { get; set; }

        public CreateModel(IWebHostEnvironment environment, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this._environment = environment;
            this._context  = context;
            _userManager = userManager;

        }
        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(Type))
            {
                return RedirectToPage("/Admin/Orders/Index");
            }

            if (Type == "purchase")
            {
                HandlePurchase();
            }
            else if (Type == "sell")
            {
                HandleSell();
            }

            return Page();
        }

        public JsonResult OnGetSearchCustomers(string phone)
        {
            var customers = _context.Customers
                .Where(c => c.PhoneNumber.Contains(phone))
                .Select(c => new { c.Id, c.FullName, c.PhoneNumber })
                .ToList();
            return new JsonResult(customers);
        }
        public JsonResult OnGetCustomerDetails(int customerId)
        {
            var customer = _context.Customers
                .Where(c => c.Id == customerId)
                .Select(c => new { c.Id, c.FullName, c.PhoneNumber, c.Email , c.Address, c.accumulated_point})
                .FirstOrDefault();

            if (customer != null)
            {
                return new JsonResult(new { success = true, customer });
            }
            return new JsonResult(new { success = false, message = "Customer not found" });
        }

        public JsonResult OnGetSearchProducts(string barcode)
        {
            var products = _context.Products
                .Where(p => p.Barcode.Contains(barcode))
                .Select(p => new { p.Id, p.ProductName })
                .ToList();
            return new JsonResult(products);
        }


/*        public IActionResult OnGetProductImage(string fileName)
        {
            string imageFullPath = Path.Combine(_environment.WebRootPath, "products", fileName);
            Console.WriteLine(imageFullPath);

            if (!System.IO.File.Exists(imageFullPath))
            {
                return NotFound();
            }
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            string mimeType;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    mimeType = "image/jpeg";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                default:
                    mimeType = "application/octet-stream";
                    break;
            }
            var imageFileStream = new FileStream(imageFullPath, FileMode.Open, FileAccess.Read);
            return File(imageFileStream, mimeType);
        }*/
        public JsonResult OnGetProductDetails(int productId)
        {
            var product = _context.Products
                 .Where(p => p.Id == productId)
                 .Select(p => new { p.ProductName, p.Barcode, p.ImageFileName , p.TypePriceId, p.Quantity})
                 .FirstOrDefault();
            var typePrice = _context.TypePrices.Find(product.TypePriceId);
            if (product != null)
            {
                string imageUrl = Url.Content($"~/products/{product.ImageFileName}");
                return new JsonResult(new
                {
                    product.ProductName,
                    product.Barcode,
                    product.Quantity,
                    typePrice.Type,
                    imageUrl,
                    success = true
                });
            }
            return new JsonResult(new { success = false, message = "product not found" });
        }
      
        public JsonResult OnGetSearchPromotions(string promotionCode)
        {
            var promotions = _context.Promotions
                  .Where(p => p.Code.Contains(promotionCode))
                  .Select(p => new { p.Id,p.Code, p.DiscountPercentage, p.FixedDiscountAmount,
                      p.StartDate, p.EndDate })
                  .ToList();
            return new JsonResult(promotions);
        }
        public JsonResult OnGetPromotionDetails(int promotionId)
        {
            var promotion = _context.Promotions
                .Where(p => p.Id == promotionId)
                .Select(p => new { p.Code, p.DiscountPercentage, p.FixedDiscountAmount })
                .FirstOrDefault();

            if (promotion != null)
            {
                return new JsonResult(promotion);
            }
            return new JsonResult(new { success = false, message = "Promotion not found" });
        }

        public JsonResult OnGetCalculatePrice(int productId)
		{
            var product = _context.Products.Find(productId);

            var typePrice = _context.TypePrices.Find(product.TypePriceId);

			if (product == null || product.TypePriceId == null)
			{
				return new JsonResult(new { success = false, message = "Product or TypePrice not found" });
			}

			// Lấy các thông số cần thiết từ Product
			double priceProcessing = product.PriceProcessing;
			double priceStone = product.PriceStone;
			double weight = product.Weight;

			// Lấy các thông số từ TypePrice
			double sellPricePerGram = typePrice.SellPricePerGram;

			// Tính toán giá (Price)
			double capitalPrice = sellPricePerGram * weight + priceProcessing + priceStone;
			double price = capitalPrice * product.PriceRate;

			return new JsonResult(new { success = true, price = price });
		}

        public JsonResult OnGetApplyPromotion(string promotionCode)
        {
            var promotionService = new PromotionService(_context);

            if (promotionService.IsPromotionValid(promotionCode))
            {
                double originalPrice = 100.0; // Ví dụ giá trị ban đầu
                double discountAmount = promotionService.CalculateDiscountAmount(promotionCode, originalPrice);

                return new JsonResult(new { success = true, discountAmount });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Invalid promotion code or promotion not active" });
            }
        }
        public string errorMessage = "";
        public string successMessage = "";
        public async Task<IActionResult> OnPostAsync(List<int> productIds, List<int> quantities)
        {
            double discount = double.Parse(Request.Form["discount"]);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                errorMessage = "User is not logged in.";
                return Page();
            }
            // Lưu email người dùng vào biến
            string createdByEmail = user.Email;

            //save the new product in database
            Order newOrder = new Order()
            {
                Date = DateTime.Now,
                CustomerID = CustomerId,
                Type = "Sell",
                Discount = discount,
                CreatedBy = createdByEmail,
            };
            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            if (productIds.Count > 0)
            {
                // Example logic to process the order and update inventory
                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = _context.Products.Find(productIds[i]);
                    var typePrice = _context.TypePrices.Find(product.TypePriceId);
                    if (product != null && quantities[i] > 0)
                    {
                        if (product.Quantity >= quantities[i])
                        {
                            var newQuantity = product.Quantity - quantities[i];
                            var unitPrice = (typePrice.SellPricePerGram * product.Weight + product.PriceStone + product.PriceProcessing) * product.PriceRate;
                            OrderDetail newOrdDetail = new OrderDetail()
                            {
                                OrderID = newOrder.Id,
                                ProductID = product.Id,
                                Quantity = quantities[i],
                                UnitPrice = unitPrice
                            };

                            
                            _context.OrderDetails.Add(newOrdDetail);

                            product.Quantity = newQuantity;
                            _context.SaveChanges();

                        }

                        else
                        {
                            ModelState.AddModelError("", $"Not enough stock for {product.ProductName}");
                            return Page();
                        }
                    }
                }
            }else
            {
                ModelState.AddModelError("", $"Not enough stock for product");
                errorMessage = "Not enough stock for product";
                return Page();
            }
            
           
            successMessage = "Order created successfully";

            return Redirect("/Admin/Orders/Create");
        }
        private void HandlePurchase()
        {
            ViewData["OrderType"] = "Purchase Order";
        }

        private void HandleSell()
        {
            ViewData["OrderType"] = "Sell Order";
        }
    }
}