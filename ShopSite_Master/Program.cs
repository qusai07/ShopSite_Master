using MyShopSite.Startup;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------
builder.Services.AddMyShopServices(builder.Configuration);
builder.Services.AddMyShopCors(builder.Configuration);

// -------------------- Build app --------------------
var app = builder.Build();

// -------------------- Middleware --------------------
app.UseMyShopMiddleware();

// -------------------- Endpoints --------------------
app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
