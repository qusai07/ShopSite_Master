
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using MyShop_Site.Models.ResponseModels;
using MyShop_Site.Repo.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using LoginRequestModel = MyShop_Site.Models.Common.LoginRequestModel;

namespace MyShop_Site.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly MasterService _masterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProtectedSessionStorage _protectedStorage;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        private readonly ISecureCookieService _cookieService;



        public AuthenticationService(
            MasterService masterService, IHttpContextAccessor httpContextAccessor,
            ProtectedSessionStorage protectedStorage,
            CustomAuthenticationStateProvider authStateProvider)

        {
            _masterService = masterService;
            _httpContextAccessor = httpContextAccessor;
            _protectedStorage = protectedStorage;
            _authStateProvider = authStateProvider;
        }
        public async Task<UserInfoResponseModel> GetProfileAsync()
        {
            try
            {
                // Get the stored token from your secure cookie
                var stored = await _protectedStorage.GetAsync<string>("auth_data");
                if (!stored.Success || string.IsNullOrEmpty(stored.Value)) return null;

                var authData = JsonSerializer.Deserialize<Models.Authentication.AuthenticationData>(_dataProtector.Unprotect(stored.Value));
                if (authData == null) return null;

                // Set the token in the MasterService so RequestMasterAsync uses it
                _masterService.SetAuthenticationToken(authData.Token, authData.TokenExpiry);

                // Fetch the user profile via MasterService
                var response = await _masterService.RequestMasterAsync<UserInfoResponseModel>("User/GetUser");

                if (response.IsSuccess)
                {
                    return response;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password, bool rememberMe = false)
        {
            try
            {
                var loginRequest = new LoginRequestModel
                {
                    Username = username,
                    Password = password,
                };

                var response = await _masterService.RequestMasterAsync<LoginResponseModel>(
                    "Authentication/Authenticate", loginRequest);

                if (response.IsSuccess && !string.IsNullOrEmpty(response.Token))
                {
                    var claims = new List<Claim>
                    {
                        new Claim("access_token", response.Token)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe,
                        ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8),
                        AllowRefresh = true
                    };

                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        await httpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            claimsPrincipal,
                            authProperties);
                    }

                    // Save token in ProtectedSessionStorage
                    await _protectedStorage.SetAsync("auth_token", response.Token);
                    await _protectedStorage.SetAsync("token_expiry", DateTime.UtcNow.AddHours(1).ToString("O"));

                    // Optionally store refresh token
                    await _protectedStorage.SetAsync("refresh_token", response.Token);


                    await _authStateProvider.MarkUserAsAuthenticatedAsync(claimsPrincipal);

                    return new AuthenticationResult
                    {
                        IsSuccess = true,
                        Message = "Authentication successful",
                        Token = response.Token,
                        TokenExpiry = DateTime.UtcNow.AddHours(1)
                    };
                }

                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Message = response.Message ?? "Authentication failed"
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Message = "An error occurred during authentication"
                };
            }
        }

        //public async Task<bool> RefreshTokenAsync()
        //{
        //    try
        //    {
        //        var authData = _cookieService.GetCookie<AuthenticationData>("auth_data");
        //        if (authData?.RefreshToken == null)
        //            return false;

        //        var refreshRequest = new { RefreshToken = authData.RefreshToken };
        //        var response = await _masterService.RequestMasterAsync<LoginResponseModel>("Authentication/Refresh", refreshRequest);

        //        if (response.IsSuccess && !string.IsNullOrEmpty(response.Token))
        //        {
        //            authData.Token = response.Token;
        //            authData.TokenExpiry = DateTime.UtcNow.AddHours(1);
        //            authData.RefreshToken = response.Token ?? authData.RefreshToken;

        //            _cookieService.SetSecureCookie("auth_data", authData.Token);
        //            await _authStateProvider.MarkUserAsAuthenticatedAsync(authData);

        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public async Task LogoutAsync()
        {
            try
            {
                _cookieService.ClearAuthenticationCookie();
                await _authStateProvider.MarkUserAsLoggedOutAsync();
            }
            catch (Exception ex)
            {
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

        public async Task<UserInfoResponseModel> GetCurrentUserAsync()
        {
            var authData = _cookieService.GetCookie<Models.Authentication.AuthenticationData>("auth_data");
            if (authData == null)
                return new UserInfoResponseModel { IsSuccess = false };

            // You can fetch full user details from Master API if needed
            return new UserInfoResponseModel
            {
                //UserId = authData.UserId,
                //Username = authData.Username,
                IsSuccess = true
            };
        }

        public Task<bool> RefreshTokenAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetCurrentTokenAsync()
        {
            throw new NotImplementedException();
        }
    }
}
