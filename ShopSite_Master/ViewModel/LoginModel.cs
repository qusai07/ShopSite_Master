using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyShop_Site.Helper;
using MyShop_Site.Repo.Interfaces;
using MyShop_Site.Services;
using System.ComponentModel.DataAnnotations;

namespace MyShop_Site.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthenticationService _authService;
        private readonly ISecureCookieService _cookieService;
        private readonly ILogger<LoginModel> _logger;
        private readonly IDataProtector _dataProtector;
        private string errorMessage = string.Empty;
        public bool showPassword = false;
        public bool isLoading = false;

        public LoginModel(IDataProtector dataProtector ,IAuthenticationService authService, ISecureCookieService cookieService, ILogger<LoginModel> logger)
        {
            _authService = authService;
            _cookieService = cookieService;
            _logger = logger;
            _dataProtector = dataProtector;
        }
       
        [BindProperty]
        public LoginRequestModel LoginRequest { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var authResult = await _authService.AuthenticateAsync(LoginRequest.Username, LoginRequest.Password);
            if (!authResult.IsSuccess)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            var protectedToken = _dataProtector.Protect(authResult.Token);

            Response.Cookies.Append("auth_data", protectedToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            // Redirect to Blazor Server main page
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> HandleLogin()
        {
            isLoading = true;
            errorMessage = string.Empty;

            try
            {
                var result = await _authService.AuthenticateAsync(LoginRequest.Username, LoginRequest.Password,false);
                if (result.IsSuccess)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    errorMessage = result.Message ?? "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = "An error occurred while signing in. Please try again.";
            }
            finally
            {
                isLoading = false;
            }
            return Page();

        }

        public void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }
    }


    public class LoginRequestModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
