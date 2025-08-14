using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyShop_Site.Models;
using MyShop_Site.Repo.Interfaces;
using System.Security.Claims;

namespace MyShop_Site.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProtectedSessionStorage _protectedSessionStorage;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;


        public CustomAuthenticationStateProvider(
           IHttpContextAccessor httpContextAccessor,
           ProtectedSessionStorage protectedSessionStorage,
           ILogger<CustomAuthenticationStateProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _protectedSessionStorage = protectedSessionStorage;
            _logger = logger;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            throw new NotImplementedException();
        }

        //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        //{
        //    try
        //    {
        //        var httpContext = _httpContextAccessor.HttpContext;
        //        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        //            var authdata  = await _
        //        var authData = await _cookieService.GetCookie<AuthenticationData>("auth_data");

        //        if (authData != null &&
        //            !string.IsNullOrEmpty(authData.Token) &&
        //            authData.TokenExpiry > DateTime.UtcNow)
        //        {
        //            // User is authenticated via ASP.NET Core cookies
        //            return new AuthenticationState(httpContext.User);
        //        }

        //        // Return anonymous state
        //        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning(ex, "Error reading authentication state");
        //        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //    }
        //}

        public async Task MarkUserAsAuthenticatedAsync(Models.Authentication.AuthenticationData authData)
        {
            var claims = new[]
            {
                //new Claim(ClaimTypes.NameIdentifier, authData.UserId),
                //new Claim(ClaimTypes.Name, authData.Username),
                new Claim("token", authData.Token),
                new Claim("token_expiry", authData.TokenExpiry.ToString("O"))
            };

            var identity = new ClaimsIdentity(claims, "cookie");
            var principal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
    }
}