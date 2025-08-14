namespace MyShopSite.Startup
{
    public static class MiddlewareConfig
    {
        public static void UseGlobalMiddleware(this WebApplication app)
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
        }
    }
} 
