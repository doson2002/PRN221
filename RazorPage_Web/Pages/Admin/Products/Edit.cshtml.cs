using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage_Web.DAL;
using RazorPage_Web.Models;

namespace RazorPage_Web.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
		private readonly IWebHostEnvironment environment;
		private readonly AppDbContext context;

        [BindProperty]
        public ProductDTO ProductDTO { get; set; } = new ProductDTO();

        public Product Product { get; set; } = new Product();

        public string errorMessage = "";
        public string successMessage = "";

		public EditModel(IWebHostEnvironment environment, AppDbContext context)
        {
			this.environment = environment;
			this.context = context;
		}
        public void OnGet(int? id)
        {
            if(id == null)
            {
                Response.Redirect("/Admin/Products/Index");
                return;
            }
            var product = context.Products.Find(id);
            if(product == null)
            {
				Response.Redirect("/Admin/Products/Index");
				return;
			}
            ProductDTO.ProductName = product.ProductName;
            ProductDTO.Barcode = product.Barcode;
            ProductDTO.Description = product.Description;
            ProductDTO.Weight = product.Weight;
            ProductDTO.Quantity = product.Quantity;
            ProductDTO.PriceProcessing = product.PriceProcessing;
            ProductDTO.PriceStone = product.PriceStone;
            ProductDTO.PriceRate = product.PriceRate;
            ProductDTO.TypePriceId = product.TypePriceId;
            Product = product;

        }
        public void OnPost(int? id)
        {
            if (id == null)
            {
				Response.Redirect("/Admin/Products/Index");
				return;
			}
            if(!ModelState.IsValid)
            {
                errorMessage = "Please provide all the required fields";
                return;
            }
            var product = context.Products.Find(id);
            if(product == null)
            {
				Response.Redirect("/Admin/Products/Index");
				return;
			}
            //Update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if(ProductDTO.ImageFile != null)
            {
				newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				newFileName += Path.GetExtension(ProductDTO.ImageFile!.FileName);

				string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
				using (var stream = System.IO.File.Create(imageFullPath))
				{
					ProductDTO.ImageFile.CopyTo(stream);
				}
                //delete the old image
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
			}
            //update the product in the database
            product.ProductName = ProductDTO.ProductName;
            product.Barcode = ProductDTO.Barcode;
            product.PriceProcessing = ProductDTO.PriceProcessing;
            product.PriceStone = ProductDTO.PriceStone;
            product.PriceRate = ProductDTO.PriceRate;
            product.Description = ProductDTO.Description;
            product.Quantity = ProductDTO.Quantity;
            product.Weight = ProductDTO.Weight;
            product.ImageFileName = newFileName;
            product.TypePriceId = ProductDTO.TypePriceId;


            context.SaveChanges();

            Product = product;
            successMessage = "Product updated successfully";
            Response.Redirect("/Admin/Products/Index");
        }
    }
}
