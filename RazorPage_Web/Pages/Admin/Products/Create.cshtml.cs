using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        private readonly AppDbContext context;
        [BindProperty]
        public ProductDTO productDTO { get; set; } = new ProductDTO();
        public CreateModel(IWebHostEnvironment environment, AppDbContext context)
        {
            this.environment = environment;
            this.context  = context;

        }
        public void OnGet()
        {
        }
        public string errorMessage = "";
        public string successMessage = "";
        public void OnPost() 
        { 
            if (productDTO.ImageFile == null)
            {
                ModelState.AddModelError("productDTO.ImageFile", "The image file is required");
            }        
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
                return;
            }
			// Kiểm tra trùng lặp Barcode
			bool barcodeExists = context.Products.Any(p => p.Barcode == productDTO.Barcode);
			if (barcodeExists)
			{
				ModelState.AddModelError("productDTO.Barcode", "The barcode already exists.");
				errorMessage = "The barcode already exists. Please use a different barcode.";
				return;
			}
			//save image file
			string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDTO.ImageFile!.FileName);

            // string imageFullPath = environment.WebRootPath + "/products/" + newFileName; 
            string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDTO.ImageFile.CopyTo(stream);
            }

            //save the new product in database
            Product product = new Product()
            {
                ProductName = productDTO.ProductName,
                Barcode = productDTO.Barcode,
                Description = productDTO.Description ?? "",
                Quantity = productDTO.Quantity,
                Weight = productDTO.Weight,
                PriceProcessing = productDTO.PriceProcessing,
                PriceStone = productDTO.PriceStone,
                PriceRate = productDTO.PriceRate,
                ImageFileName = newFileName,
                TypePriceId = productDTO.TypePriceId,

            };
            context.Products.Add(product);
            context.SaveChanges();
            
            
            //clear the form
            productDTO.ProductName = "";
            productDTO.Barcode = "";
            productDTO.Quantity = 0;
            productDTO.Weight = 0;
            productDTO.PriceProcessing = 0;
            productDTO.PriceStone = 0;
            productDTO.PriceRate = 0;
            productDTO.Description = "";
            productDTO.ImageFile = null;
            productDTO.TypePriceId = 0;

            ModelState.Clear();

            successMessage = "Product created successfully";

            Response.Redirect("/Admin/Products/Index");
        }
    }
}
