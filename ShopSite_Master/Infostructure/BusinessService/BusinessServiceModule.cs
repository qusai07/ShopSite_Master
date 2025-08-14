using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyShop_Site.Repo.Implementations;
using MyShop_Site.Repo.Interfaces;
using MyShop_Site.Services;
using MyShopSite.Repo.Implementations;
using MyShopSite.Repo.Interfaces;

namespace MyShopSite.Infostructure.BusinessService
{
    public static class BusinessServiceModule
    {
        public static void AddBusinessServiceModule(this IServiceCollection Services)
        {
            Services.AddScoped<IDataService, DataService>();
            Services.AddScoped<MasterService>();
            Services.AddScoped<UserService>();
            Services.AddScoped<ProductService>();
            Services.AddScoped<ProtectedSessionStorage>();
            Services.AddScoped<ISecureCookieService, SecureCookieService>();
            Services.AddHttpContextAccessor();

        }
    }
}
