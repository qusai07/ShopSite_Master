using MyShop_Site.Models.ResponseModels;
using MyShop_Site.Repo.Interfaces;
using ShopSite_Master.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyShop_Site.Models.RequestModels;
using FailedResponseModel = MyShop_Site.Models.ResponseModels.FailedResponseModel;

namespace ShopSite_Master.Services.Master
{
    public class MasterService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MasterService> _logger;
        private string _authToken = string.Empty;
        private DateTime _tokenExpiry = DateTime.MinValue;
        private readonly ISecureCookieService _cookieService;
        private readonly ITokenService _tokenService;

        public string MasterBaseUrl { get; }

        public MasterService(IHttpClientFactory httpClientFactory, ISecureCookieService cookieService, IConfiguration configuration, ILogger<MasterService> logger, ITokenService tokenService)
        {
            _cookieService = cookieService;
            _httpClient = httpClientFactory.CreateClient("MasterAPI");
            _configuration = configuration;
            _logger = logger;
            MasterBaseUrl = _configuration["MasterAPI:BaseUrl"] ?? "http://192.168.1.115:5300/SmartApp";
            _tokenService = tokenService;
        }

        public void SetAuthenticationToken(string token, DateTime expiry)
        {
            _authToken = token;
            _tokenExpiry = expiry;
        }

public async Task<T?> RequestMasterAsync<T>(string Operation, object? requestModel = null, bool isUnauthorized = false)
    {
        try
        {
            // احصل على التوكن من الكوكيز
            var token = await _cookieService.GetCookie("AuthToken");

            using HttpClient httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                _authToken = token;
                _tokenExpiry = DateTime.UtcNow.AddHours(1);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Add("MasterToken", token);
            }

            // تجهيز الـ content
            HttpContent? content = null;
            if (requestModel != null)
            {
                content = new StringContent(
                    JsonSerializer.Serialize(requestModel, requestModel.GetType()),
                    Encoding.UTF8,
                    "application/json");
            }

            // إرسال الطلب
            var httpResponse = await httpClient.GetAsync($"{MasterBaseUrl}/api/{Operation}");
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            // التعامل مع الحالة حسب status code
            if (httpResponse.IsSuccessStatusCode)
            {
                // تحويل JSON إلى الكائن المطلوب
                return JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized && !isUnauthorized)
            {
                // إعادة محاولة بعد التعامل مع التفويض
                // bool isAuth = await AuthAccountUser(); // لو عندك دالة إعادة تسجيل دخول
                return await RequestMasterAsync<T>(Operation, requestModel, true);
            }
            else
            {
                // في حال حدوث خطأ
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


    private bool IsTokenExpired()
        {
            return string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiry;
        }

    }
}
