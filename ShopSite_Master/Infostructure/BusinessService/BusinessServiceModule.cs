using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyShop_Site.Repo.Implementations;
using MyShop_Site.Repo.Interfaces;
using MyShop_Site.Services;
using MyShopSite.Repo.Implementations;
using MyShopSite.Repo.Interfaces;

namespace MyShopSite.Startup
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMyShopServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Core & Blazor
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpClient();

            // API clients
            services.AddHttpClient("MasterAPI", client =>
            {
                var baseUrl = configuration["MasterAPI:BaseUrl"] ?? "https://dev.minerets.com/ShopMaster/";
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Authentication
            services.AddScoped<CustomAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(
                sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
            services.AddScoped<IAuthenticationService, AuthenticationService>();


            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IApiClient, ApiClient>();


            // Data protection
            services.AddDataProtection();

            // Authentication & Authorization
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/access-denied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.Name = "MyShop.Auth";
                });

            services.AddAuthorization();
            services.AddControllers();

            return services;
        }
        public static IServiceCollection AddBusinessServiceModule(this IServiceCollection services)
        {
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<UserService>();
            services.AddScoped<ProductService>();

            // Browser storage
            services.AddScoped<ProtectedSessionStorage>();   

            // JWT token service & cookie
            services.AddScoped<ISecureCookieService, SecureCookieService>();

            services.AddHttpContextAccessor();

            return services; 
            // return the IServiceCollection for chaining
        }

        public static IServiceCollection AddMyShopCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalNetwork", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
