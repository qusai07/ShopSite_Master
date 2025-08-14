namespace MyShop_Site.Helper
{
    public static class TokenManager
    {
        public static void SetToken(HttpContext context, string token)
        {
            context.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(1)
            });
        }

        public static string? GetToken(HttpContext context)
        {
            return context.Request.Cookies["AuthToken"];
        }
    }

}
