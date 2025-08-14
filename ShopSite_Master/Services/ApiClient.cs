
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyShop_Site.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IJwtTokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiClient> _logger;
        private readonly NavigationManager _navigationManager;

        public ApiClient(
            IHttpClientFactory httpClientFactory,
            IJwtTokenService tokenService,
            IConfiguration configuration,
            ILogger<ApiClient> logger,
            NavigationManager navigationManager)
        {
            _httpClient = httpClientFactory.CreateClient("MasterAPI");
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
            _navigationManager = navigationManager;
        }

        public async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            return await SendRequestAsync<T>(HttpMethod.Get, endpoint);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object? data = null) where T : class
        {
            return await SendRequestAsync<T>(HttpMethod.Post, endpoint, data);
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data) where T : class
        {
            return await SendRequestAsync<T>(HttpMethod.Put, endpoint, data);
        }

        public async Task<T?> DeleteAsync<T>(string endpoint) where T : class
        {
            return await SendRequestAsync<T>(HttpMethod.Delete, endpoint);
        }

        private async Task<T?> SendRequestAsync<T>(HttpMethod method, string endpoint, object? data = null) where T : class
        {
            try
            {
                // Set authorization header
                await SetAuthorizationHeaderAsync();

                // Create request
                var request = new HttpRequestMessage(method, endpoint);

                // Add content if data is provided
                if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                // Send request
                var response = await _httpClient.SendAsync(request);

                // Handle unauthorized response
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await HandleUnauthorizedAsync();
                    return null;
                }

                // Handle successful response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    if (string.IsNullOrEmpty(responseContent))
                        return null;

                    var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result;
                }

                _logger.LogWarning("API request failed: {StatusCode} - {Endpoint}", response.StatusCode, endpoint);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making API request to {Endpoint}", endpoint);
                return null;
            }
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async Task HandleUnauthorizedAsync()
        {
            _logger.LogWarning("Received 401 Unauthorized response");
            
            // Clear token and redirect to login
            await _tokenService.DeleteTokenAsync();
            _navigationManager.NavigateTo("/login", true);
        }
    }
}
