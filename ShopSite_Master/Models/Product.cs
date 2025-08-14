
namespace MyShop_Site.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProductCategory Category { get; set; }
        public decimal Price { get; set; }
        public string Features { get; set; } = string.Empty;
        public string TechnicalSpecs { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string ShippingPolicy { get; set; } = string.Empty;
        public string ReturnPolicy { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public enum ProductCategory
    {
        Hardware,
        Products,
        Apps
    }
}
