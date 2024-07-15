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

        [BindProperty(SupportsGet = true)]
        public string SearchBarcode { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<int> ProductIds { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
        public List<ProductDetailDTO> ProductDetails { get; set; } = new List<ProductDetailDTO>();

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 2; // You can adjust the page size as needed




        [BindProperty]
        public int CustomerId { get; set; }

        [BindProperty]
        public int PromotionId { get; set; }

        public List<CustomerPolicy> CustomerPolicies { get; set; } = new List<CustomerPolicy>();

        public CreateModel(IWebHostEnvironment environment, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this._environment = environment;
            this._context = context;
            _userManager = userManager;

        }
        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
		{
            // Attempt to get Type from TempData or set it if passed in the query string
            Type = TempData["Type"] as string ?? Type;

            if (string.IsNullOrEmpty(Type))
            {
                return RedirectToPage("/Admin/Orders/Index");
            }else
            {
                // Store or update the Type in TempData and ensure it's kept for the next request
                TempData["Type"] = Type;
                TempData.Keep("Type");  // Ensure that the Type is kept for subsequent requests
            }
            // Lấy ProductIds từ TempData
            if (TempData.TryGetValue("ProductIds", out var tempProductIds))
            {
                // Ép kiểu tempProductIds về int[] và chuyển đổi thành List<int>
                ProductIds = ((int[])tempProductIds).ToList();

                // Log ProductIds để kiểm tra xem chúng có bị null hay không
                Console.WriteLine("ProductIds: " + (ProductIds != null ? string.Join(", ", ProductIds) : "null"));
            }

            CustomerPolicies = await _context.CustomerPolicies
                      .Where(p => p.PublishingStatus == "Accepted")
                      .OrderByDescending(p => p.Id)
                      .ToListAsync();

            // Handle the business logic based on Type
            if (Type == "purchase")
            {
                HandlePurchase();

                if (ProductIds != null && ProductIds.Any())
                {
                    // Load products based on productIds (without pagination)
                    IQueryable<Product> productsQuery = _context.Products.Where(p => ProductIds.Contains(p.Id));
                    ProductDetails = await LoadProductsAsync(productsQuery, 1, int.MaxValue);

                    // Transform ProductDetails to ProductDetailDTO if needed
                    var productDTOs = ProductDetails.Select(pd => new ProductDetailDTO
                    {
                        Id = pd.Id,
                        ProductName = pd.ProductName,
                        Barcode = pd.Barcode,
                        Quantity = pd.Quantity,
                        Type = pd.Type,
                        ImageUrl = pd.ImageUrl
                        // Add other properties as needed
                    }).ToList();

                    // Return JsonResult with productDTOs
                    return new JsonResult(productDTOs);
                }
                else
                {
                    // Load products with pagination (default behavior)
                    await LoadAllProductsAsync(currentPage, PageSize);
                }
            }
            else if (Type == "sell")
            {
                CurrentPage = currentPage;
                HandleSell();

                // Load products with or without pagination based on search term
                if (string.IsNullOrEmpty(SearchBarcode))
                {
                    // Load products with pagination
                    await LoadAllProductsAsync(CurrentPage, PageSize);
                }
                else
                {
                    // Load all products without pagination (filtered by searchTerm)
                    await LoadAllProductsAsync(1, int.MaxValue);
                }

            }

                // Keep TempData "Type" for another round of request
                TempData.Keep("Type");

            return Page();
        }
        public JsonResult OnGetSearchOrders(int orderID)
        {
            var order = _context.Orders
                .Where(o => o.Id.Equals(orderID))
                .Join(_context.Customers, // Bảng mà bạn muốn join
                      o => o.CustomerID, // Khóa ngoại từ OrderDetails
                      c => c.Id, // Khóa chính từ Customer
                      (o, c) => new { o.Id, o.Date, o.Discount, c.FullName }) // Chọn thông tin bạn muốn trả về
                  .ToList();

            return new JsonResult(order);
        }
        public JsonResult OnGetOrderDetails(int orderID)
        {
            var orderDetails = _context.OrderDetails
                .Where(od => od.OrderID == orderID)
                .Join(_context.Products, // Bảng mà bạn muốn join
                      od => od.ProductID, // Khóa ngoại từ OrderDetails
                      p => p.Id, // Khóa chính từ Products
                      (od, p) => new { od.Id, od.ProductID, p.Barcode, od.Quantity, od.UnitPrice }) // Chọn thông tin bạn muốn trả về
                .ToList();

            if (orderDetails != null && orderDetails.Count > 0)
            {
                ProductIds = orderDetails.Select(od => od.ProductID).Distinct().ToList();
                TempData["ProductIds"] = ProductIds; // Lưu ProductIds vào TempData
                return new JsonResult(new { success = true, orderDetails, ProductIds });
            }
            return new JsonResult(new { success = false, message = "Order detail not found" });
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
                .Select(c => new { c.Id, c.FullName, c.PhoneNumber, c.Email, c.Address, c.accumulated_point })
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
                 .Select(p => new { p.ProductName, p.Barcode, p.ImageFileName, p.TypePriceId, p.Quantity })
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
                  .Select(p => new {
                      p.Id,
                      p.Code,
                      p.DiscountPercentage,
                      p.FixedDiscountAmount,
                      p.StartDate,
                      p.EndDate
                  })
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
            // Lấy giá trị Type từ TempData
            string type = TempData["Type"] as string;

            // Nếu Type không tồn tại, trả về lỗi hoặc xử lý tương ứng
            if (string.IsNullOrEmpty(type))
            {
                return new JsonResult(new { success = false, message = "Type not specified" });
            }

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

            double pricePerGram;
            // Kiểm tra type và lấy giá phù hợp
            if (type == "sell")
            {
                pricePerGram = typePrice.SellPricePerGram;
            }
            else if (type == "purchase")
            {
                pricePerGram = typePrice.PurchasePricePerGram; // Assume this attribute exists in TypePrice
            }
            else
            {
                return new JsonResult(new { success = false, message = "Invalid type specified" });
            }

            // Tính toán giá (Price)
            double capitalPrice = pricePerGram * weight + priceProcessing + priceStone;
            double price = capitalPrice * product.PriceRate;
            // Giữ lại Type cho request tiếp theo
            TempData.Keep("Type");

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
        public async Task<IActionResult> OnPostAsync(List<int> productIds, List<int> quantities, string paymentMethod)
        {
            double discount = double.Parse(Request.Form["discount"]);
            // Lấy giá trị Type từ TempData
            string type = TempData["Type"] as string;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                errorMessage = "User is not logged in.";
                return Page();
            }
            string createdByEmail = user.Email;

          
            Order newOrder = new Order()
            {
                Date = DateTime.Now,
                CustomerID = CustomerId,
                Type = this.Type,  // Directly use the Type property bound to the class
                Discount = discount,
                CreatedBy = createdByEmail,
            };
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            decimal total = 0; // Tính toán tổng số tiền

            if (productIds.Count > 0)
            {
                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = _context.Products.Find(productIds[i]);
                    var typePrice = _context.TypePrices.Find(product.TypePriceId);
                    if (product != null && quantities[i] > 0)
                    {

                        double unitPrice = 0;

                        if (type == "sell")
                        {
                            if (product.Quantity >= quantities[i])
                            {
                                product.Quantity -= quantities[i];  // Subtract for Sell
                                unitPrice = (typePrice.SellPricePerGram * product.Weight + product.PriceStone + product.PriceProcessing) * product.PriceRate;

                            }
                            else
                            {
                                ModelState.AddModelError("", $"Not enough stock for {product.ProductName}");
                                return Page();
                            }


                        }
                        else if (type == "purchase")
                        {
                            product.Quantity += quantities[i];  // Add for Purchase
                            unitPrice = (typePrice.PurchasePricePerGram * product.Weight + product.PriceStone + product.PriceProcessing) * product.PriceRate;

                        }
                        OrderDetail newOrdDetail = new OrderDetail()
                        {
                            OrderID = newOrder.Id,
                            ProductID = product.Id,
                            Quantity = quantities[i],
                            UnitPrice = unitPrice
                        };

                       await _context.OrderDetails.AddAsync(newOrdDetail);
                        // Cộng dồn vào total
                        total += (decimal)unitPrice * quantities[i];

                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Not enough stock for product");
                errorMessage = "Not enough stock for product";
                return Page();
            }

            // Trừ đi discount
            total -= (decimal)discount;
            await _context.SaveChangesAsync();
            // Giữ lại Type cho request tiếp theo
            TempData.Keep("Type");
            // Xử lý lựa chọn thanh toán
            if (paymentMethod == "momo")
            {
                // Chuyển hướng đến trang quét mã QR với orderId làm query parameter
                return Redirect($"/Admin/Orders/ScanMoMoQR?orderId={newOrder.Id}&total={total}");
            }
            else if (paymentMethod == "cash")
            {
                // Xử lý thanh toán bằng tiền mặt
                // Đoạn code xử lý tạo order và order detail như bạn đã viết ở trên
                HttpContext.Session.SetString("SuccessMessage", "Order created successfully");
                // Redirect sau khi xử lý thành công
                return Redirect("/Admin/Orders/Index");
            }
            return Page();
          
        }
        private void HandlePurchase()
        {
            ViewData["OrderType"] = "Purchase Order";
        }

        private void HandleSell()
        {
            ViewData["OrderType"] = "Sell Order";
        }


        private async Task<List<ProductDetailDTO>> LoadProductsAsync(IQueryable<Product> productsQuery, int pageNumber, int pageSize)
        {
            // Tính toán tổng số sản phẩm (sau khi đã áp dụng điều kiện tìm kiếm nếu có)
            int totalProducts = await productsQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Sắp xếp, phân trang và lấy dữ liệu
            var productsWithDetails = await productsQuery
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(product => new
                {
                    product.Id,
                    product.ProductName,
                    product.Barcode,
                    product.ImageFileName,
                    product.Quantity,
                    product.TypePriceId
                })
                .ToListAsync();

            var productDetails = productsWithDetails.Select(product =>
            {
                var typePrice = _context.TypePrices.Find(product.TypePriceId);
                return new ProductDetailDTO
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Barcode = product.Barcode,
                    Quantity = product.Quantity,
                    Type = typePrice?.Type, // Kiểm tra null trước khi truy cập
                    ImageUrl = Url.Content($"~/products/{product.ImageFileName}")
                };
            }).ToList();

            return productDetails;
        }
        private async Task LoadAllProductsAsync(int pageNumber, int pageSize)
        {
            IQueryable<Product> productsQuery = _context.Products;

            // Thêm điều kiện Where để lọc kết quả tìm kiếm nếu searchTerm không rỗng
            if (!string.IsNullOrEmpty(SearchBarcode))
            {
                if(Type == "sell")
                {
                    productsQuery = productsQuery.Where(p => p.Barcode.Contains(SearchBarcode));

                }else if(Type == "purchase")
                {
                    
                }
            }
            if (ProductIds != null && ProductIds.Any())
            {
                // Load products based on productIds (without pagination)
                productsQuery = productsQuery.Where(p => ProductIds.Contains(p.Id));

            }

            // Load products based on productsQuery
            ProductDetails = await LoadProductsAsync(productsQuery, pageNumber, pageSize);
        }

        public async Task<IActionResult> OnGetLoadProductsAsync(string searchTerm)
        {
            IQueryable<Product> productsQuery = _context.Products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery.Where(p => p.Barcode.Contains(searchTerm));
            }

            if (Type == "purchase" && ProductIds != null && ProductIds.Count > 0)
            {
                productsQuery = productsQuery.Where(p => ProductIds.Contains(p.Id));
            }

            var productsWithDetails = await productsQuery
                .OrderByDescending(p => p.Id)
                .Select(product => new
                {
                    product.Id,
                    product.ProductName,
                    product.Barcode,
                    product.ImageFileName,
                    product.Quantity,
                    product.TypePriceId
                })
                .ToListAsync();

            var productDetails = productsWithDetails.Select(product =>
            {
                var typePrice = _context.TypePrices.Find(product.TypePriceId);
                return new ProductDetailDTO
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Barcode = product.Barcode,
                    Quantity = product.Quantity,
                    Type = typePrice?.Type, // Kiểm tra null trước khi truy cập
                    ImageUrl = Url.Content($"~/products/{product.ImageFileName}")
                };
            }).ToList();

            return new JsonResult(productDetails); // Trả về dữ liệu sản phẩm dưới dạng JSON
        }

        public JsonResult OnGetGetPolicy(int policyId)
        {
            var policy = _context.CustomerPolicies.FirstOrDefault(p => p.Id == policyId);
            if (policy == null)
            {
                return new JsonResult(new { success = false, message = "Policy not found" });

            }
            return new JsonResult(new { success = true, policy });
        }

    }
}