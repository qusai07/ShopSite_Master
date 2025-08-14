
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyShop_Site.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJwtTokenService _tokenService;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;

        public CustomAuthenticationStateProvider(
           IJwtTokenService tokenService,
           ILogger<CustomAuthenticationStateProvider> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenService.GetTokenAsync();

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            // فقط إنشاء مستخدم Authenticated بدون Claims
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "apiauth");
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationState(principal);
        }




        public async Task MarkUserAsAuthenticatedAsync(string token)
        {
            await _tokenService.SetTokenAsync(token);
            var authState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _tokenService.DeleteTokenAsync();
            var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
    }
}
