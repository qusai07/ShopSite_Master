namespace MyShopSite.Startup
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseMyShopMiddleware(this WebApplication app)
        {
            app.UseCors("AllowLocalNetwork");

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request from {context.Connection.RemoteIpAddress} to {context.Request.Path}");
                await next();
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<MyShopSite.Middleware.ExceptionMiddleware>();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
using Microsoft.AspNetCore.Builder;

namespace MyShopSite.Startup
{
    public static class MiddlewareConfig
    {
        public static IApplicationBuilder UseMyShopMiddleware(this IApplicationBuilder app)
        {
            // Configure the HTTP request pipeline
            if (!app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("AllowLocalNetwork");

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
