namespace MyShop_Site.Repo.Interfaces
{
    public interface ISecureCookieService
    {
        // حفظ قيمة نصية
        Task SetSecureCookie(string key, string value);

        // حفظ قيمة Generic
        Task SetSecureCookie<T>(string key, T value) where T : class;

        // استرجاع قيمة نصية
        Task<string?> GetCookie(string key);

        // استرجاع قيمة Generic
        Task<T?> GetCookie<T>(string key) where T : class;

        // حذف الكوكي
        Task DeleteCookie(string key);

        // إعداد كوكي المصادقة
        Task SetAuthenticationCookie(string userId, bool rememberMe = false);

        // مسح كوكي المصادقة
        Task ClearAuthenticationCookie();

        // التحقق من وجود كوكي صالح
        Task<bool> ValidateSecureCookie(string key);

        // كوكي "تذكرني"
        Task SetRememberMeCookie(int userId, string username, int expireDays = 30);
        Task<(int UserId, string Username)?> GetRememberMeData();

        // إعداد كوكي سلة التسوق
        Task SetCartCookie(System.Collections.Generic.List<int> productIds);

        // إعداد كوكي تفضيلات المستخدم
        Task SetUserPreferences(object preferences);

        // مسح جميع كوكيات التطبيق
        Task ClearAllAppCookies();
    }
}
