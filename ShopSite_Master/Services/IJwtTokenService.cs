
using System;
using System.Threading.Tasks;

namespace MyShop_Site.Services
{
    public interface IJwtTokenService
    {
        Task SetTokenAsync(string token);
        Task<string?> GetTokenAsync();
        Task DeleteTokenAsync();
        Task<bool> IsTokenValidAsync();
    }
}
