using Microsoft.AspNetCore.Http.HttpResults;
using MyShop_Site.Models.RequestModels;
using MyShop_Site.Models.ResponseModels;
using MyShop_Site.Repo.Interfaces;
using ShopSite_Master.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using FailedResponseModel = MyShop_Site.Models.ResponseModels.FailedResponseModel;

namespace ShopSite_Master.Services.Master
{
    public class MasterService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<MasterService> _logger; 
        private string? _authToken;
        private DateTime _tokenExpiry;
        private readonly ISecureCookieService _cookieService;

        private string MasterBaseUrl => _configuration["MasterAPI:BaseUrl"] ?? "https://dev.minerets.com/ShopMaster";

        public MasterService(ISecureCookieService cookieService, IConfiguration configuration, ILogger<MasterService> logger)
        {
            _cookieService = cookieService;
            _configuration = configuration;
            _logger = logger;
        }

 
        public async Task<T?> RequestMasterAsync<T>(string Operation, object? requestModel = null, bool isUnauthorized = false)
        {
            try
            {
                var token = await _cookieService.GetCookie("AuthToken");
                if (IsTokenExpired())
                {
                    //SetAuthenticationToken(_authToken, _tokenExpiry);

                    //_logger.LogWarning("Token expired, cannot proceed with request.");
                    //return default;
                }

                using HttpClient httpClient = new HttpClient();
                if (!string.IsNullOrEmpty(token))
                {
                    _authToken = token;
                    _tokenExpiry = DateTime.UtcNow.AddHours(1);

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Add("MasterToken", token);
                }

                HttpContent? content = null;
                if (requestModel != null)
                {
                    content = new StringContent(
                        JsonSerializer.Serialize(requestModel, requestModel.GetType()),
                        Encoding.UTF8,
                        "application/json");
                }

                var httpResponse = await httpClient.PostAsync($"{MasterBaseUrl}/api/{Operation}" , content);
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized && !isUnauthorized)
                {
                    // bool isAuth = await AuthAccountUser(); 
                    return await RequestMasterAsync<T>(Operation, requestModel, true);
                }
                else
                {
                    Console.WriteLine($"Request failed ({httpResponse.StatusCode}): {responseString}");
                    return default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RequestMasterAsync: {ex.Message}");
                return default;
            }
        }
        public void SetAuthenticationToken(string token, DateTime expiry)
        {
            _authToken = token;
            _tokenExpiry = expiry;
        }
        private bool IsTokenExpired()
        {
            return string.IsNullOrEmpty(_authToken) || DateTime.Now >= _tokenExpiry;
        }

    }
}
