
namespace MyShop_Site.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        public List<OrderAddOn> AddOns { get; set; } = new();
        public BillingCycle BillingCycle { get; set; }
        public int UserCount { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public BillingDetails BillingDetails { get; set; } = null!;
    }

    public class OrderAddOn
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int AddOnId { get; set; }
        public AddOn AddOn { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class BillingDetails
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public enum BillingCycle
    {
        Monthly,
        Yearly
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Failed,
        Cancelled
    }
}
