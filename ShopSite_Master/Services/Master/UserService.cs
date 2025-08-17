using Microsoft.AspNetCore.Components;
using MyShop_Site.Models.Common;
using MyShop_Site.Models.ResponseModels;
using ShopSite_Master.Services.Implementations;
using ShopSite_Master.Services.Interfaces;

namespace ShopSite_Master.Services.Master
{
    public class UserService
    {
        private readonly MasterService _masterService;
        private readonly ILogger<UserService> _logger;
        private readonly ITokenService _tokenService;
        private readonly CustomAuthenticationStateProvider _authStateProvider;


        public UserService(MasterService masterService, ILogger<UserService> logger , ITokenService tokenService, CustomAuthenticationStateProvider authStateProvider)
        {
            _masterService = masterService;
            _logger = logger;
            _tokenService = tokenService;
            _authStateProvider = authStateProvider;
        }
        //public async Task<RegisterResponseModel> RegisterAsync(RegisterRequestModel registerRequest)
        //{
        //    try
        //    {
        //        var response = await _masterService.RequestMasterAsync<RegisterResponseModel>("auth/register", registerRequest);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Registration failed for user: {Username}", registerRequest.Username);
        //        return new RegisterResponseModel
        //        {
        //            IsSuccess = false,
        //            Message = "Registration failed. Please try again."
        //        };
        //    }
        //}

        //public async Task<UserInfoResponseModel> GetUserInfoAsync(int userId)
        //{
        //    try
        //    {
        //        var response = await _masterService.RequestMasterAsync<UserInfoResponseModel>($"users/{userId}");
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get user info for ID: {UserId}", userId);
        //        return new UserInfoResponseModel
        //        {
        //            IsSuccess = false,
        //            Message = "Failed to retrieve user information."
        //        };
        //    }
        //}

        //public async Task<BaseResponseModel> UpdateUserAsync(int userId, object updateRequest)
        //{
        //    try
        //    {
        //        var response = await _masterService.RequestMasterAsync<BaseResponseModel>($"users/{userId}", updateRequest);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to update user: {UserId}", userId);
        //        return new BaseResponseModel
        //        {
        //            IsSuccess = false,
        //            Message = "Failed to update user information."
        //        };
        //    }
        //}
        public async Task<UserInfoResponseModel?> GetProfileAsync()
        {
            try
            {
                var response = await _masterService.RequestMasterAsync<UserInfoResponseModel>("User/GetUser");

                if (response is UserInfoResponseModel userInfo)
                {
                    return userInfo;
                }

                _logger.LogWarning("Failed to get user profile. Response is not UserInfoResponseModel.");
                return null;
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
                // Update authentication state
                await _authStateProvider.MarkUserAsLoggedOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
        }

    }
}
