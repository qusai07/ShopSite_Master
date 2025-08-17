using MyShopSite.Startup;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------
builder.Services.AddApplicationServices(builder.Configuration);


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
