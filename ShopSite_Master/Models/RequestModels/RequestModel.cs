using System.Net;
using System.Text.Json;
using System.Text;
using MyShop_Site.Models.ResponseModels; // ✅ نستخدم نفس IResponseModel هنا

namespace MyShop_Site.Models.RequestModels
{
    public interface IRequestModel
    {
        HttpContent? GetContect();
    }

    public abstract class JsonRequestModel : IRequestModel
    {
        public HttpContent GetContect()
        {
            return new StringContent(JsonSerializer.Serialize(this, GetType()), Encoding.UTF8, "application/json");
        }
    }

    public class EmptyRequestModel : IRequestModel
    {
        public HttpContent? GetContect() => null;
    }

    public class EmptyIdRequestModel : EmptyRequestModel
    {
        public Guid ID { get; set; }
    }

    public class JsonIdRequestModel : JsonRequestModel
    {
        public Guid ID { get; set; }
    }

    // ✅ يستعمل IResponseModel الموحد
    public class FileResponseModel : IResponseModel
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }

    public class UnauthorizedResponseModel : IResponseModel
    {
        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; } = "Unauthorized";
    }

    public class FailedResponseModel : IResponseModel
    {
        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorDetail { get; set; }
    }
}
