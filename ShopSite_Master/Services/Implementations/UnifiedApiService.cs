
using MyShop_Site.Services.Abstractions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyShop_Site.Services.Implementations
{
    public class UnifiedApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ISecureStorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UnifiedApiService> _logger;

        public UnifiedApiService(
            HttpClient httpClient,
            ISecureStorageService storageService,
            IConfiguration configuration,
            ILogger<UnifiedApiService> logger)
        {
            _httpClient = httpClient;
            _storageService = storageService;
            _configuration = configuration;
            _logger = logger;

            var baseUrl = _configuration["MasterAPI:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint) where T : class
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync(endpoint);
            return await ProcessResponseAsync<T>(response);
        }

        public async Task<T> PostAsync<T>(string endpoint, object? data = null) where T : class
        {
            await SetAuthHeaderAsync();
            var json = data != null ? JsonSerializer.Serialize(data) : string.Empty;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            return await ProcessResponseAsync<T>(response);
        }

        public async Task<T> PutAsync<T>(string endpoint, object data) where T : class
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            return await ProcessResponseAsync<T>(response);
        }

        public async Task<T> DeleteAsync<T>(string endpoint) where T : class
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync(endpoint);
            return await ProcessResponseAsync<T>(response);
        }

        public void SetAuthenticationToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearAuthentication()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task SetAuthHeaderAsync()
        {
            var authData = await _storageService.GetSecureAsync<Models.Authentication.AuthenticationData>("auth_data");
            if (authData?.Token != null && authData.TokenExpiry > DateTime.UtcNow)
            {
                SetAuthenticationToken(authData.Token);
            }
        }

        private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? Activator.CreateInstance<T>();
            }

            _logger.LogWarning("API call failed: {StatusCode} - {Content}", response.StatusCode, content);
            throw new HttpRequestException($"API call failed: {response.StatusCode}");
        }
    }
}
