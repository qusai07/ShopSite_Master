
using System.Text.Json.Serialization;

namespace MyShop_Site.Models.ResponseModels
{
    public class UserInfoResponseModel : BaseResponseModel
    {
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        [JsonPropertyName("UserName ")]
        public string UserName { get; set; }

        [JsonPropertyName("EmailAddress ")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("contactName")]
        public string ContactName { get; set; }

        [JsonPropertyName("MobileNumber ")]
        public string MobileNumber { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("companySize")]
        public string CompanySize { get; set; }

        [JsonPropertyName("industry")]
        public string Industry { get; set; }
        [JsonPropertyName("FullName")]
        public string FullName { get; set; }
        [JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }
    }

    public class LoginResponseModel : BaseResponseModel
    {

        [JsonPropertyName("token")]
        public string Token { get; set; }

    }

    public class RegisterResponseModel : BaseResponseModel
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

    }
}
