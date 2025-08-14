using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using MyShop_Site.Repo.Implementations;
using MyShop_Site.Repo.Interfaces;
using MyShop_Site.Services;
using MyShopSite.Startup;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------

// Core & Razor/Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// HttpClient for APIs
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("MasterAPI", client =>
{
    var masterApiUrl = builder.Configuration["MasterAPI:BaseUrl"] ?? "https://dev.minerets.com/ShopMaster/";
    client.BaseAddress = new Uri(masterApiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
// Scoped default HttpClient for Blazor components
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());

// Repositories
builder.Services.AddScoped<ISecureCookieService, SecureCookieService>();

// JWT Token Service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// API Client
builder.Services.AddScoped<IApiClient, ApiClient>();

// Custom services
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    provider => provider.GetRequiredService<CustomAuthenticationStateProvider>()
);

// Authentication service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Data Protection
builder.Services.AddDataProtection();

// Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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
        options.ReturnUrlParameter = "returnUrl";
    });

builder.Services.AddAuthorization();

// Optional: IHttpContextAccessor for cookies in services
builder.Services.AddHttpContextAccessor();

// Add controllers
builder.Services.AddControllers();

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true
);

// -------------------- Build app --------------------
var app = builder.Build();

// Middleware
app.UseGlobalMiddleware();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();
