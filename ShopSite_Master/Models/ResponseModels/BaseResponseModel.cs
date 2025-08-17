
using System.Text.Json.Serialization;

namespace MyShop_Site.Models.ResponseModels
{
    public interface IResponseModel
    {
        bool IsSuccess { get; set; }
        string? Message { get; set; }
    }

    public class BaseResponseModel : IResponseModel
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }
    }

    public class BaseResponseModel<T> : BaseResponseModel
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class ListResponseModel<T> : BaseResponseModel
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new List<T>();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }

    public class EmptyResponseModel : IResponseModel
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }

    public class FailedResponseModel : IResponseModel
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string ErrorDetail { get; set; } = string.Empty;
    }
}
