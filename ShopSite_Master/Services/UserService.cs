
using MyShop_Site.Models.Common;
using MyShop_Site.Models.ResponseModels;

namespace MyShop_Site.Services
{
    public class UserService
    {
        private readonly MasterService _masterService;
        private readonly ILogger<UserService> _logger;

        public UserService(MasterService masterService, ILogger<UserService> logger)
        {
            _masterService = masterService;
            _logger = logger;
        }
        public async Task<RegisterResponseModel> RegisterAsync(RegisterRequestModel registerRequest)
        {
            try
            {
                var response = await _masterService.RequestMasterAsync<RegisterResponseModel>("auth/register", registerRequest);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for user: {Username}", registerRequest.Username);
                return new RegisterResponseModel
                {
                    IsSuccess = false,
                    Message = "Registration failed. Please try again."
                };
            }
        }

        public async Task<UserInfoResponseModel> GetUserInfoAsync(int userId)
        {
            try
            {
                var response = await _masterService.RequestMasterAsync<UserInfoResponseModel>($"users/{userId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user info for ID: {UserId}", userId);
                return new UserInfoResponseModel
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve user information."
                };
            }
        }

        public async Task<BaseResponseModel> UpdateUserAsync(int userId, object updateRequest)
        {
            try
            {
                var response = await _masterService.RequestMasterAsync<BaseResponseModel>($"users/{userId}", updateRequest);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user: {UserId}", userId);
                return new BaseResponseModel
                {
                    IsSuccess = false,
                    Message = "Failed to update user information."
                };
            }
        }

        public void Logout()
        {
            _masterService.ClearAuthentication();
        }
    }
}
