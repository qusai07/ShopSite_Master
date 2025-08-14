using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyShop_Site.Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyShop_Site.Repo.Implementations
{
    public class SecureCookieService : ISecureCookieService
    {
        private readonly ProtectedLocalStorage _protectedStorage;
        private readonly IDataProtector _dataProtector;
        private readonly ILogger<SecureCookieService> _logger;
        private readonly IConfiguration _configuration;

        public SecureCookieService(
            ProtectedLocalStorage protectedStorage,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<SecureCookieService> logger,
            IConfiguration configuration)
        {
            _protectedStorage = protectedStorage;
            _dataProtector = dataProtectionProvider.CreateProtector("MyShop.SecureCookies");
            _logger = logger;
            _configuration = configuration;
        }
        public void SetTokenCookie(string token, int expireMinutes = 60)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,       // JS can't read it
                Secure = true,         // Only HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("AuthToken", token, options);
        }
        public async Task SetSecureCookie(string key, string value)
        {
            try
            {
                var protectedValue = _dataProtector.Protect(value);
                await _protectedStorage.SetAsync(key, protectedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting secure cookie: {CookieKey}", key);
            }
        }

        public async Task SetSecureCookie<T>(string key, T value) where T : class
        {
            try
            {
                var jsonValue = JsonSerializer.Serialize(value);
                await SetSecureCookie(key, jsonValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing and setting secure cookie: {CookieKey}", key);
            }
        }

        public async Task<string?> GetCookie(string key)
        {
            try
            {
                var result = await _protectedStorage.GetAsync<string>(key);
                if (!result.Success || string.IsNullOrEmpty(result.Value))
                    return null;

                return _dataProtector.Unprotect(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading secure cookie: {CookieKey}", key);
                await DeleteCookie(key);
                return null;
            }
        }

        public async Task<T?> GetCookie<T>(string key) where T : class
        {
            try
            {
                var json = await GetCookie(key);
                if (string.IsNullOrEmpty(json)) return null;

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deserializing secure cookie: {CookieKey}", key);
                await DeleteCookie(key);
                return null;
            }
        }

        public async Task DeleteCookie(string key)
        {
            try
            {
                await _protectedStorage.DeleteAsync(key);
                _logger.LogInformation("Secure cookie deleted: {CookieKey}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting secure cookie: {CookieKey}", key);
            }
        }

        public async Task SetAuthenticationCookie(string userId, bool rememberMe = false)
        {
            var authData = new AuthCookieData
            {
                UserId = userId,
                IssuedAt = DateTime.UtcNow,
                SessionId = Guid.NewGuid().ToString()
            };

            var expiryDays = rememberMe ? 30 : 1;
            await SetSecureCookie("auth_session", authData);
            await SetSecureCookie("csrf_token", Guid.NewGuid().ToString());
        }

        public async Task ClearAuthenticationCookie()
        {
            await DeleteCookie("auth_session");
            await DeleteCookie("csrf_token");
        }

        public async Task<bool> ValidateSecureCookie(string key)
        {
            var value = await GetCookie(key);
            return !string.IsNullOrEmpty(value);
        }

        public async Task SetRememberMeCookie(int userId, string username, int expireDays = 30)
        {
            var rememberData = new
            {
                UserId = userId,
                Username = username,
                CreatedAt = DateTime.UtcNow
            };

            await SetSecureCookie("RememberMe", rememberData);
        }

        public async Task<(int UserId, string Username)?> GetRememberMeData()
        {
            try
            {
                var data = await GetCookie<dynamic>("RememberMe");
                if (data != null)
                {
                    var jsonElement = (JsonElement)data;
                    return (
                        jsonElement.GetProperty("UserId").GetInt32(),
                        jsonElement.GetProperty("Username").GetString() ?? ""
                    );
                }
            }
            catch { }

            return null;
        }

        public async Task SetCartCookie(List<int> productIds)
        {
            await SetSecureCookie("ShoppingCart", productIds);
        }

        public async Task SetUserPreferences(object preferences)
        {
            await SetSecureCookie("UserPreferences", preferences);
        }

        public async Task ClearAllAppCookies()
        {
            var cookiesToDelete = new[] { "RememberMe", "ShoppingCart", "UserPreferences", "auth_session", "csrf_token" };
            foreach (var cookie in cookiesToDelete)
                await DeleteCookie(cookie);
        }
    }

    public class AuthCookieData
    {
        public string UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public string SessionId { get; set; }
    }
}
