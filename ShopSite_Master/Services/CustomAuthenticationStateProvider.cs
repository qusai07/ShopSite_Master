
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
            try
            {
                var token = await _tokenService.GetTokenAsync();
                
                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Parse JWT token to extract claims
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                // Check if token is expired
                if (jsonToken.ValidTo <= DateTime.UtcNow)
                {
                    await _tokenService.DeleteTokenAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Create claims from JWT
                var claims = new List<Claim>();
                
                // Add standard claims from JWT
                foreach (var claim in jsonToken.Claims)
                {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }

                // Ensure we have required claims
                if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    // Add user ID from 'sub' claim if it exists
                    var subClaim = claims.FirstOrDefault(c => c.Type == "sub");
                    if (subClaim != null)
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, subClaim.Value));
                    }
                }

                if (!claims.Any(c => c.Type == ClaimTypes.Name))
                {
                    // Add name from various possible claims
                    var nameClaim = claims.FirstOrDefault(c => c.Type == "name" || c.Type == "username" || c.Type == "unique_name");
                    if (nameClaim != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
                    }
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                var principal = new ClaimsPrincipal(identity);

                return new AuthenticationState(principal);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading authentication state");
                await _tokenService.DeleteTokenAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticatedAsync(string token)
        {
            try
            {
                await _tokenService.SetTokenAsync(token);
                
                // Parse token and create authentication state
                var authState = await GetAuthenticationStateAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking user as authenticated");
            }
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            try
            {
                await _tokenService.DeleteTokenAsync();
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking user as logged out");
            }
        }
    }
}
