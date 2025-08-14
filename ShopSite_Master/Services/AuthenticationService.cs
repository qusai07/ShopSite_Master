using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MyShop_Site.Models.Authentication;
using MyShop_Site.Models.ResponseModels;
using MyShop_Site.Models.Common;
using MyShop_Site.Repo.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;


namespace MyShop_Site.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiClient _apiClient;
        private readonly IJwtTokenService _tokenService;
        private readonly CustomAuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly MasterService _masterService;

        public AuthenticationService(
            IApiClient apiClient,
            IJwtTokenService tokenService,
            CustomAuthenticationStateProvider authStateProvider,
            NavigationManager navigationManager,
            ILogger<AuthenticationService> logger, MasterService masterService)
        {
            _apiClient = apiClient;
            _tokenService = tokenService;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
            _logger = logger;
            _masterService = masterService;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password, bool rememberMe = false)
        {

            var response = await _masterService.RequestMasterAsync<LoginResponseModel>(
              "Authentication/Authenticate",
              new LoginRequestModel { Username = username, Password = password });

            if (!response.IsSuccess || string.IsNullOrEmpty(response.Token))
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Message = response?.Message ?? "Authentication failed"
                };

            // ✅ تخزين التوكن وتحديث AuthenticationStateProvider
            await _tokenService.SetTokenAsync(response.Token);
            await _authStateProvider.MarkUserAsAuthenticatedAsync(response.Token);

            return new AuthenticationResult
            {
                IsSuccess = true,
                Message = "Login successful",
                Token = response.Token
            };

        }
     


        public async Task<UserInfoResponseModel> GetProfileAsync()
        {
            try
            {
                var response = await _masterService.RequestMasterAsync<UserInfoResponseModel>("User/GetUser");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                // Clear token from the token service
                await _tokenService.DeleteTokenAsync();

                // Update authentication state
                await _authStateProvider.MarkUserAsLoggedOutAsync();

                // Redirect to login page
                _navigationManager.NavigateTo("/login", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated == true;
        }


        public async Task<string> GetCurrentUserIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        public async Task<UserInfoResponseModel?> GetCurrentUserAsync()
        {
            // This method should ideally fetch user profile using the API client
            return await GetProfileAsync();
        }

        public async Task<bool> RefreshTokenAsync()
        {
            // Implement logic to refresh the token using the API client and IJwtTokenService
            // This is a placeholder, actual implementation depends on API capabilities
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            // Assuming the API has a Refresh endpoint that accepts the current token
            // and returns a new token.
            var refreshResponse = await _apiClient.PostAsync<LoginResponseModel>("Authentication/Refresh", new { Token = token });

            if (refreshResponse?.IsSuccess == true && !string.IsNullOrEmpty(refreshResponse.Token))
            {
                await _tokenService.SetTokenAsync(refreshResponse.Token);
                await _authStateProvider.MarkUserAsAuthenticatedAsync(refreshResponse.Token); // Re-authenticate with new token
                return true;
            }

            // If refresh fails, logout the user
            await LogoutAsync();
            return false;
        }

        public async Task<string?> GetCurrentTokenAsync()
        {
            return await _tokenService.GetTokenAsync();
        }
    }
}