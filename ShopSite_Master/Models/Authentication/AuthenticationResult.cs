
using System;

namespace MyShop_Site.Models.Authentication
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
    }
}
