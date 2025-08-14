
using System.Threading.Tasks;

namespace MyShop_Site.Services
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string endpoint) where T : class;
        Task<T?> PostAsync<T>(string endpoint, object? data = null) where T : class;
        Task<T?> PutAsync<T>(string endpoint, object data) where T : class;
        Task<T?> DeleteAsync<T>(string endpoint) where T : class;
    }
}
