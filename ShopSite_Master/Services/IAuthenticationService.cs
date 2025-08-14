
using MyShop_Site.Models.Authentication;
using MyShop_Site.Models.ResponseModels;

namespace MyShop_Site.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string username, string password, bool rememberMe = false);
        Task<bool> RefreshTokenAsync();
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string> GetCurrentUserIdAsync();
        Task<UserInfoResponseModel> GetCurrentUserAsync();
        Task<string?> GetCurrentTokenAsync();
    }
}
