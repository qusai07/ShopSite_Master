
using MyShop_Site.Models.Common;
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
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfoResponseModel? User { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserInfoResponseModel? User { get; set; }
    public string? Token { get; set; }
    public DateTime? TokenExpiry { get; set; }
}
