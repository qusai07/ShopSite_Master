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

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            string token = null;
            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                $"http://192.168.0.15/ShopMaster/api/Authentication/Authenticate",
                JsonContent.Create(new { username, password }));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                token = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            return true;

        }



        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated == true;
        }
    }
}