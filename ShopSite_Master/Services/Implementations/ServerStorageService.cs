
using MyShop_Site.Repo.Interfaces;
using MyShop_Site.Services.Abstractions;

namespace MyShop_Site.Services.Implementations
{
    public class ServerStorageService : ISecureStorageService
    {
        private readonly ISecureCookieService _cookieService;

        public ServerStorageService(ISecureCookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            return null; // Task.FromResult(await _cookieService.GetCookie<T>(key));
        }

        public Task SetAsync<T>(string key, string value, TimeSpan? expiry = null) where T : class
        {
            _cookieService.SetSecureCookie(key, value);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cookieService.DeleteCookie(key);
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _cookieService.ClearAuthenticationCookie();
            return Task.CompletedTask;
        }

        public Task SetSecureAsync<T>(string key, string value, TimeSpan? expiry = null) where T : class
        {
            _cookieService.SetSecureCookie(key, value);
            return Task.CompletedTask;
        }

        public Task<T?> GetSecureAsync<T>(string key) where T : class
        {
            return null; // Task.FromResult(_cookieService.GetCookie<T>(key));
        }

        public Task SetSecureAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            throw new NotImplementedException();
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
