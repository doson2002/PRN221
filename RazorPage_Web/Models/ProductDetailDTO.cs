namespace RazorPage_Web.Models
{
    public class ProductDetailDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
    }
}
