using MyShop_Site.Models.Common;
using MyShop_Site.Models.ResponseModels;
using MyShop_Site.Repo.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyShop_Site.Services
{
    public class MasterService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MasterService> _logger;
        private string _authToken = string.Empty;
        private DateTime _tokenExpiry = DateTime.MinValue;
        private readonly ISecureCookieService _cookieService;


        public string MasterBaseUrl { get; }

        public  MasterService(ISecureCookieService cookieService,HttpClient httpClient, IConfiguration configuration, ILogger<MasterService> logger)
        {
            _cookieService = cookieService;
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            MasterBaseUrl = _configuration["MasterAPI:BaseUrl"] ?? "http://192.168.0.15/ShopMaster";
            //var token = _cookieService("AuthToken");
            //if (!string.IsNullOrEmpty(token))
            //{
            //    _authToken = token;
            //    _tokenExpiry = DateTime.UtcNow.AddHours(1);
            //}

        }

        public void SetAuthenticationToken(string token, DateTime expiry)
        {
            _authToken = token;
            _tokenExpiry = expiry;
        }

        public void ClearAuthentications()
        {
            _authToken = string.Empty;
            _tokenExpiry = DateTime.MinValue;
            _cookieService.DeleteCookie("auth_data");
        }

        public async Task<T> RequestMasterAsync<T>(string operation, object? requestModel = null, bool isRetry = false) where T : IResponseModel, new()
        {
            var responseModel = new T();

            try
            {
                if (IsTokenExpired() && !isRetry)
                {
                    // Handle token refresh here if necessary
                }

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{MasterBaseUrl}/api/{operation}");

                if (!string.IsNullOrEmpty(_authToken) && !isRetry)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                }

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (requestModel != null)
                {
                    var json = JsonSerializer.Serialize(requestModel);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                using var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(LoginResponseModel))
                    {
                        return (T)(object)new LoginResponseModel
                        {
                            Token = responseContent,
                            IsSuccess = true,
                            Message = "Success"
                        };
                    }
                    else
                    {
                        var deserialized = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return deserialized ?? new T { IsSuccess = false, Message = "Empty response from server." };
                    }
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized && !isRetry)
                {
                    return await RequestMasterAsync<T>(operation, requestModel, true);
                }

                responseModel.IsSuccess = false;
                responseModel.Message = $"{response.StatusCode}: {responseContent}";
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize JSON for operation: {Operation}", operation);
                responseModel.IsSuccess = false;
                responseModel.Message = "Invalid JSON response.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request to Master API failed for operation: {Operation}", operation);
                responseModel.IsSuccess = false;
                responseModel.Message = $"Unexpected error: {ex.Message}";
            }

            return responseModel;
        }


        private bool IsTokenExpired()
        {
            return string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiry;
        }

        public void ClearAuthentication()
        {
            _authToken = string.Empty;
            _tokenExpiry = DateTime.MinValue;
            _cookieService.DeleteCookie("AuthToken");
        }

    }
}
