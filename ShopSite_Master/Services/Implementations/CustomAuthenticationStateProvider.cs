
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using MyShop_Site.Repo.Interfaces;
using ShopSite_Master.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopSite_Master.Services.Implementations
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISecureCookieService _cookieService;
        private const string TokenKey = "jwt_token";
        private readonly ProtectedSessionStorage _sessionStorage;


        private readonly ILogger<CustomAuthenticationStateProvider> _logger;

        public CustomAuthenticationStateProvider(
            ISecureCookieService cookieService,
            ProtectedSessionStorage sessionStorage,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _cookieService = cookieService;
            _sessionStorage = sessionStorage;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _cookieService.GetCookie("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "User") };
                var identity = new ClaimsIdentity(claims, "apiauth");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            // مستخدم غير مصدق
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }


        public async Task MarkUserAsAuthenticated(string username, string token)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, "apiauth");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _cookieService.ClearAllAppCookies();
            NotifyAuthenticationStateChanged(Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
            _logger.LogInformation("User logged out successfully.");
        }

    }
}
