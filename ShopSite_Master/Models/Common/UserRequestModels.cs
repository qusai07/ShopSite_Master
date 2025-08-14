using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyShop_Site.Models.Common
{
    public class LoginRequestModel
    {
        [JsonPropertyName("username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class RegisterRequestModel
    {
        [JsonPropertyName("companyName")]
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }

        [JsonPropertyName("contactName")]
        [Required(ErrorMessage = "Contact Name is required")]
        public string ContactName { get; set; }

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("country")]
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [JsonPropertyName("companySize")]
        [Required(ErrorMessage = "Company Size is required")]
        public string CompanySize { get; set; }

        [JsonPropertyName("industry")]
        public string Industry { get; set; }

        [JsonPropertyName("username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [JsonPropertyName("confirmPassword")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [JsonPropertyName("acceptTerms")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
        public bool AcceptTerms { get; set; }
    }
}
