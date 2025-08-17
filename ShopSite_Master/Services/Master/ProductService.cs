using MyShop_Site.Models.ResponseModels;

namespace ShopSite_Master.Services.Master
{
    public class ProductService
    {
        private readonly MasterService _masterService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(MasterService masterService, ILogger<ProductService> logger)
        {
            _masterService = masterService;
            _logger = logger;
        }

        //public async Task<ProductListResponseModel> GetProductsByCategoryAsync(string category, int page = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        var requestParams = new
        //        {
        //            category,
        //            page,
        //            pageSize
        //        };

        //        var response = await _masterService.RequestMasterAsync<ProductListResponseModel>("products/category", requestParams);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get products for category: {Category}", category);
        //        return new ProductListResponseModel
        //        {
        //            IsSuccess = false,
        //            Message = "Failed to retrieve products."
        //        };
        //    }
        //}

        //public async Task<ProductDetailsResponseModel> GetProductByIdAsync(int productId)
        //{
        //    try
        //    {
        //        var response = await _masterService.RequestMasterAsync<ProductDetailsResponseModel>($"products/{productId}");
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get product: {ProductId}", productId);
        //        return new ProductDetailsResponseModel
        //        {
        //            IsSuccess = false,
        //            Message = "Failed to retrieve product details."
        //        };
        //    }
        //}

        //public async Task<ProductListResponseModel> SearchProductsAsync(string searchTerm, string category = null, int page = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        var requestParams = new
        //        {
        //            searchTerm,
        //            category,
        //            page,
        //            pageSize
        //        };

        //        var response = await _masterService.RequestMasterAsync<ProductListResponseModel>("products/search", requestParams);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to search products: {SearchTerm}", searchTerm);
        //        return new ProductListResponseModel
        //        {
        //            IsSuccess = false,
        //            Message = "Search failed. Please try again."
        //        };
        //    }
        //}

        //public async Task<ProductListResponseModel> GetFeaturedProductsAsync()
        //{
        //    try
        //    {
        //        var response = await _masterService.RequestMasterAsync<ProductListResponseModel>("products/featured");
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get featured products");
        //        return new ProductListResponseModel
        //        {
        //            IsSuccess = false,
        //            Message = "Failed to retrieve featured products."
        //        };
        //    }
        //}
    }
}
