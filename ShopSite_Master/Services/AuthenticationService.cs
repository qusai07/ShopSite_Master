using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using MyShop_Site.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using MyShop_Site.Repo.Interfaces;
using LoginRequestModel = MyShop_Site.Models.Common.LoginRequestModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;


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
            try
            {
                LoginRequestModel loginRequest = new ()
                {
                    Username = username,
                    Password = password,
                };

                var response = await _masterService.RequestMasterAsync<LoginResponseModel>("Authentication/Authenticate", loginRequest);

                if (response?.IsSuccess == true && !string.IsNullOrEmpty(response.Token))
                {
                    // Store token securely
                    await _tokenService.SetTokenAsync(response.Token);

                    // Update authentication state
                    // This part assumes CustomAuthenticationStateProvider has a method to set the token and authenticate
                    // If it relies on ClaimsPrincipal, we might need to create one from the token.
                    // For now, assuming it can handle the token directly for authentication state.
                    await _authStateProvider.MarkUserAsAuthenticatedAsync(response.Token);

                    return new AuthenticationResult
                    {
                        IsSuccess = true,
                        Message = "Authentication successful",
                        Token = response.Token,
                        // TokenExpiry should ideally be parsed from the JWT itself
                        TokenExpiry = DateTime.UtcNow.AddHours(1)
                    };
                }

                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Message = response?.Message ?? "Authentication failed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Message = "An error occurred during authentication"
                };
            }
        }

        public async Task<UserInfoResponseModel?> GetProfileAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync<UserInfoResponseModel>("User/GetUser");
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