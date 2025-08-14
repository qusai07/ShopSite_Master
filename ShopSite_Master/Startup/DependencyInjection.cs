using Microsoft.Extensions.Configuration;
using MyShopSite.Infostructure.BusinessService;

namespace MyShopSite.Startup
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddBusinessServiceModule();
        }

        public static void AddCoresPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalNetwork", policy =>
                {
                    policy.WithOrigins("")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
    }
}
