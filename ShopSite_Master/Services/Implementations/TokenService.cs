using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyShop_Site.Repo.Interfaces;
using ShopSite_Master.Services.Interfaces;

namespace ShopSite_Master.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly ISecureCookieService _cookieService;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ILogger<TokenService> _logger;
        private string _authToken = string.Empty;
        private const string TokenKey = "AuthToken";

        public TokenService(
            ISecureCookieService cookieService,
            ProtectedSessionStorage sessionStorage,
            ILogger<TokenService> logger)
        {
            _cookieService = cookieService;
            _sessionStorage = sessionStorage;
            _logger = logger;
        }

        public void SetAuthenticationToken(string token)
        {
            _authToken = token;
        }
        public async Task SaveTokenAsync(string token)
        {
            try
            {
                await _cookieService.SetTokenCookie(token);

                // Fallback to ProtectedSessionStorage
                await _sessionStorage.SetAsync(token, token);

                _logger.LogInformation("Token saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save token.");
                throw;
            }
        }
    }
}
