
using System.ComponentModel.DataAnnotations;

namespace MyShop_Site.Models.Common
{
    public class LoginRequestModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
