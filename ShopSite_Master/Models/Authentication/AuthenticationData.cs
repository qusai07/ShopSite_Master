namespace MyShop_Site.Models.Authentication
{
    public class AuthenticationData
    {
        //public string UserId { get; set; } = string.Empty;
        //public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}
