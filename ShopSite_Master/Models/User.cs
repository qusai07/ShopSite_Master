namespace MyShop_Site.Models
{
    public class User
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public CompanySize CompanySize { get; set; }
        public string Industry { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    public enum CompanySize
    {
        LessThan5,
        Between5And20,
        Between20And50,
        Between50And250,
        MoreThan250
    }
}