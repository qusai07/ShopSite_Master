
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MyShop_Site.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly ProtectedSessionStorage _protectedStorage;
        private readonly IDataProtector _dataProtector;
        private readonly ILogger<JwtTokenService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TOKEN_KEY = "jwt_token";
        private const string TOKEN_EXPIRY_KEY = "jwt_token_expiry";

        public JwtTokenService(
            ProtectedSessionStorage protectedStorage,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<JwtTokenService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _protectedStorage = protectedStorage;
            _dataProtector = dataProtectionProvider.CreateProtector("MyShop.JwtTokens");
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SetTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    await DeleteTokenAsync();
                    return;
                }

                // Parse token to get expiry
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                var expiry = jsonToken.ValidTo;

                // Encrypt and store token
                var protectedToken = _dataProtector.Protect(token);
                await _protectedStorage.SetAsync(TOKEN_KEY, protectedToken);
                await _protectedStorage.SetAsync(TOKEN_EXPIRY_KEY, expiry.ToString("O"));

                // Set secure HTTP-only cookie as backup
                SetSecureCookie(token, expiry);

                _logger.LogInformation("JWT token stored securely");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing JWT token");
                throw;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                // First try to get from ProtectedSessionStorage
                var result = await _protectedStorage.GetAsync<string>(TOKEN_KEY);
                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    var token = _dataProtector.Unprotect(result.Value);
                    
                    // Check if token is still valid
                    if (await IsTokenValidInternalAsync(token))
                    {
                        return token;
                    }
                }

                // If not found or invalid, try to get from cookie
                var cookieToken = GetTokenFromCookie();
                if (!string.IsNullOrEmpty(cookieToken) && await IsTokenValidInternalAsync(cookieToken))
                {
                    // Restore to session storage
                    await SetTokenAsync(cookieToken);
                    return cookieToken;
                }

                // Token not found or invalid
                await DeleteTokenAsync();
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error retrieving JWT token");
                await DeleteTokenAsync();
                return null;
            }
        }

        public async Task DeleteTokenAsync()
        {
            try
            {
                await _protectedStorage.DeleteAsync(TOKEN_KEY);
                await _protectedStorage.DeleteAsync(TOKEN_EXPIRY_KEY);
                ClearSecureCookie();
                _logger.LogInformation("JWT token cleared");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing JWT token");
            }
        }

        public async Task<bool> IsTokenValidAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        private async Task<bool> IsTokenValidInternalAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return false;

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                
                // Check if token is expired (with 5 minute buffer)
                return jsonToken.ValidTo > DateTime.UtcNow.AddMinutes(5);
            }
            catch
            {
                return false;
            }
        }

        private void SetSecureCookie(string token, DateTime expiry)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var protectedToken = _dataProtector.Protect(token);
                    var options = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = expiry,
                        Path = "/"
                    };

                    httpContext.Response.Cookies.Append("auth_token", protectedToken, options);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error setting secure cookie");
            }
        }

        private string? GetTokenFromCookie()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.Request.Cookies.TryGetValue("auth_token", out var cookieValue) == true)
                {
                    return _dataProtector.Unprotect(cookieValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading token from cookie");
            }
            return null;
        }

        private void ClearSecureCookie()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Response.Cookies.Delete("auth_token", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Path = "/"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error clearing secure cookie");
            }
        }
    }
}
