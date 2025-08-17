using MyShop_Site.Models.Authentication;
using MyShop_Site.Models.ResponseModels;

namespace ShopSite_Master.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task <AuthenticationResult> AuthenticateAsync(string username, string password);
        Task<bool> IsAuthenticatedAsync();
    }
}
