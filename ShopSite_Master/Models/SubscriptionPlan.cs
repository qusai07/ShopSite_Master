
namespace MyShop_Site.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public List<string> Features { get; set; } = new();
        public PlanTier Tier { get; set; }
        public bool IsPopular { get; set; }
    }

    public enum PlanTier
    {
        Basic,
        Standard,
        Premium
    }

    public class AddOn
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public AddOnType Type { get; set; }
    }

    public enum AddOnType
    {
        Feature,
        Storage,
        Users,
        Support
    }
}
