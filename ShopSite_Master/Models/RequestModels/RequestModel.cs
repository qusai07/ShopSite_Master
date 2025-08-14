using System.Net;
using System.Text.Json;
using System.Text;

namespace MyShop_Site.Models.RequestModels
{
    public interface IResponseModel
    {
    }
    public interface IRequestModel
    {
        HttpContent GetContect();
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
        public HttpContent GetContect()
        {
            return null;
        }
    }
    public class EmptyResponseModel : IResponseModel
    {
    }

    public class EmptyIdRequestModel : EmptyRequestModel
    {
        public Guid ID { get; set; }
    }

    public class JsonIdRequestModel : JsonRequestModel
    {
        public Guid ID { get; set; }

    }

    public class FileResponseModel : IResponseModel
    {
        public string FilePath { get; set; }
    }

    public class UnauthorizedResponseModel : IResponseModel
    {

    }
    public class FailedResponseModel : IResponseModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDetail { get; set; }

    }
}