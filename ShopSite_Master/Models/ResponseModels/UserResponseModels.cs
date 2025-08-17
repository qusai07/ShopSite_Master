
using System.Text.Json.Serialization;
using MyShop_Site.Models.RequestModels;
namespace MyShop_Site.Models.ResponseModels
{
    public class UserInfoResponseModel : IResponseModel
    {
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        [JsonPropertyName("UserName")]
        public string UserName { get; set; }

        [JsonPropertyName("EmailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("MobileNumber")]
        public string MobileNumber { get; set; }

        [JsonPropertyName("FullName")]
        public string FullName { get; set; }

        [JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }

        // Required by IResponseModel
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }

    public class LoginResponseModel : IResponseModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class RegisterResponseModel : BaseResponseModel
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

    }
}
