
namespace MyShop_Site.Services.Abstractions
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint) where T : class;
        Task<T> PostAsync<T>(string endpoint, object? data = null) where T : class;
        Task<T> PutAsync<T>(string endpoint, object data) where T : class;
        Task<T> DeleteAsync<T>(string endpoint) where T : class;
        void SetAuthenticationToken(string token);
        void ClearAuthentication();
    }

    public interface IStorageService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task RemoveAsync(string key);
        Task ClearAsync();
    }

    public interface ISecureStorageService : IStorageService
    {
        Task SetSecureAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task<T?> GetSecureAsync<T>(string key) where T : class;
    }
}
