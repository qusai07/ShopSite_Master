namespace ShopSite_Master.Pages
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using MyShop_Site.Repo.Interfaces;
    using ShopSite_Master.Services.Interfaces;
    using System.Security.Claims;

    public class DoLoginModel : PageModel
    {
        private readonly Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly ISecureCookieService _cookieService;

        public DoLoginModel(Services.Interfaces.IAuthenticationService authenticationService,ISecureCookieService cookieService)
        {
            _authenticationService = authenticationService;
            _cookieService = cookieService;

        }
        public async Task<IActionResult> OnGetAsync(string username , string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return Redirect("/login");

            var result = await _authenticationService.AuthenticateAsync(username, password);

            if (!result.IsSuccess)
                return Redirect("/login");
            await _cookieService.SetTokenCookie(result.Token);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in using cookie authentication
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
                });

            return Redirect("/profile");
        }
    }

}
