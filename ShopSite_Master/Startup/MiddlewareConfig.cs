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
