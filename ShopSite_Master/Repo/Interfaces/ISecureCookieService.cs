namespace MyShop_Site.Repo.Interfaces
{
    public interface ISecureCookieService
    {
        Task SetSecureCookie(string key, string value);
        Task SetTokenCookie(string key);

        // Generic
        Task SetSecureCookie<T>(string key, T value) where T : class;

        Task<string?> GetCookie(string key);

        // Generic
        Task<T?> GetCookie<T>(string key) where T : class;

        Task DeleteCookie(string key);

        Task SetAuthenticationCookie(string userId, bool rememberMe = false);

        Task ClearAuthenticationCookie();

        Task<bool> ValidateSecureCookie(string key);

        Task SetRememberMeCookie(int userId, string username, int expireDays = 30);
        Task<(int UserId, string Username)?> GetRememberMeData();

        Task SetCartCookie(System.Collections.Generic.List<int> productIds);

        Task SetUserPreferences(object preferences);

        Task ClearAllAppCookies();
    }
}
