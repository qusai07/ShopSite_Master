using Microsoft.IdentityModel.Logging;
using MyShop_Site.Models.Common;
using MyShop_Site.Models.RequestModels;
using MyShop_Site.Models.ResponseModels;
using MyShop_Site.Repo.Interfaces;
using ShopSite_Master.Services.Implementations;
using ShopSite_Master.Services.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EmptyResponseModel = MyShop_Site.Models.RequestModels.EmptyResponseModel;
using FailedResponseModel = MyShop_Site.Models.RequestModels.FailedResponseModel;
using IResponseModel = MyShop_Site.Models.RequestModels.IResponseModel;

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
            MasterBaseUrl = _configuration["MasterAPI:BaseUrl"] ?? "http://192.168.0.15/ShopMaster";
            _tokenService = tokenService;
        }

        public void SetAuthenticationToken(string token, DateTime expiry)
        {
            _authToken = token;
            _tokenExpiry = expiry;
        }
        public async Task<IResponseModel> RequestMasterAsync<T>(string Operation, object requestModel = null, bool isUnauthorized = false) where T : IResponseModel
        {
            IResponseModel responseModel;

            try
            { 
                var token = await _cookieService.GetCookie("AuthToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _authToken = token;
                    _tokenExpiry = DateTime.UtcNow.AddHours(1);
                }
                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                httpClient.DefaultRequestHeaders.Add("MasterToken", token);

                using HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                         $"{MasterBaseUrl}/api/{Operation}",
                         requestModel == null ? null : new StringContent(JsonSerializer.Serialize(requestModel, requestModel.GetType()), Encoding.UTF8, "application/json"));

                switch (httpResponseMessage.StatusCode)
                {
                    case HttpStatusCode.OK:
                        switch (typeof(T).Name)
                        {
                            case nameof(EmptyResponseModel):
                                responseModel = new EmptyResponseModel();
                                break;
                            default:
                                responseModel = JsonSerializer.Deserialize<T>(await httpResponseMessage.Content.ReadAsStringAsync());
                                break;
                        }
                        break;
                    case HttpStatusCode.BadRequest:
                        responseModel = new FailedResponseModel()
                        {
                            ErrorCode = await httpResponseMessage.Content.ReadAsStringAsync(),
                            ErrorDetail = await httpResponseMessage.Content.ReadAsStringAsync()

                        };
                        break;
                    case HttpStatusCode.Unauthorized:
                        if (isUnauthorized)
                        {
                            responseModel = responseModel = new FailedResponseModel()
                            {
                                ErrorCode = "Unauthorized"
                            };
                        }
                        else
                        {
                            //bool isAuth = await AuthAccountUser();
                            responseModel = await RequestMasterAsync<T>(Operation, requestModel, true);
                        }
                        break;
                    case HttpStatusCode.InternalServerError:
                        responseModel = new FailedResponseModel()
                        {
                            ErrorCode = "ServerError",
                            ErrorDetail = await httpResponseMessage.Content.ReadAsStringAsync()
                        };
                        break;
                    default:
                        responseModel = new FailedResponseModel()
                        {
                            ErrorCode = "UnknownError",
                            ErrorDetail = await httpResponseMessage.Content.ReadAsStringAsync()
                        };
                        break;
                }
            }
            catch (Exception ex)
            {
                responseModel = new FailedResponseModel()
                {
                    ErrorCode = "UnknownError",
                    ErrorDetail = ex.Message
                };
            }
            return responseModel;
        }


        private bool IsTokenExpired()
        {
            return string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiry;
        }

    }
}
