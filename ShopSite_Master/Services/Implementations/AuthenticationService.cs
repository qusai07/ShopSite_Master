using MyShop_Site.Models.Authentication;
using MyShop_Site.Models.Common;
using MyShop_Site.Models.ResponseModels;
using ShopSite_Master.Services.Interfaces;
using ShopSite_Master.Services.Master;
using System.Net.Http.Headers;

namespace ShopSite_Master.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly CustomAuthenticationStateProvider _authStateProvider;
        private readonly MasterService _masterService;

        public AuthenticationService(CustomAuthenticationStateProvider authStateProvider,MasterService masterService)
        {
            _authStateProvider = authStateProvider;
            _masterService = masterService;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string UserName, string password)
        {
            using HttpClient httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                "https://dev.minerets.com/ShopMaster/api/Authentication/Authenticate",
                JsonContent.Create(new { UserName, password }));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new AuthenticationResult
                {
                    IsSuccess = true,
                    Token = content
                };
            }
            else
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Message = $"Login failed: {content}"
                };
            }
        }
        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated == true;
        }
    }
}