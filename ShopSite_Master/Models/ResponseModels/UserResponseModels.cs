
using System.Text.Json.Serialization;
using MyShop_Site.Models.RequestModels;
namespace MyShop_Site.Models.ResponseModels
{
    using System.Text.Json.Serialization;

    public class UserInfoResponseModel
    {
        [JsonPropertyName("id")]
        public Guid ID { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }

    public class LoginResponseModel : IResponseModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class RegisterResponseModel : ResponseModel
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

    }
}
