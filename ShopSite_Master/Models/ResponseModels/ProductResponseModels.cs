
using System.Text.Json.Serialization;

namespace MyShop_Site.Models.ResponseModels
{
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("features")]
        public List<string> Features { get; set; } = new List<string>();

        [JsonPropertyName("technicalSpecs")]
        public Dictionary<string, string> TechnicalSpecs { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("images")]
        public List<string> Images { get; set; } = new List<string>();

        [JsonPropertyName("videos")]
        public List<string> Videos { get; set; } = new List<string>();

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("reviewCount")]
        public int ReviewCount { get; set; }

        [JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonPropertyName("shippingPolicy")]
        public string ShippingPolicy { get; set; }

        [JsonPropertyName("returnPolicy")]
        public string ReturnPolicy { get; set; }

        [JsonPropertyName("securityInfo")]
        public string SecurityInfo { get; set; }

        [JsonPropertyName("customizationOptions")]
        public string CustomizationOptions { get; set; }

        [JsonPropertyName("supportResources")]
        public string SupportResources { get; set; }

        [JsonPropertyName("roadmap")]
        public string Roadmap { get; set; }
    }

    public class ProductListResponseModel : ListResponseModel<ProductDto>
    {
    }

    public class ProductDetailsResponseModel : BaseResponseModel<ProductDto>
    {
    }
}
