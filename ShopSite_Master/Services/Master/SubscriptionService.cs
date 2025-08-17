using MyShop_Site.Models;

namespace ShopSite_Master.Services.Master
{
    public class SubscriptionService
    {

        public SubscriptionService()
        {
        }

        //public async Task<List<SubscriptionPlan>> GetSubscriptionPlansAsync()
        //{
      
        //}

        //public async Task<SubscriptionPlan?> GetSubscriptionPlanAsync(int planId)
        //{

        //}

        //public async Task<List<AddOn>> GetAddOnsAsync()
        //{
        //}

        public decimal CalculateTotal(SubscriptionPlan plan, List<AddOn> addOns, BillingCycle cycle, int userCount = 1)
        {
            var planPrice = cycle == BillingCycle.Monthly ? plan.MonthlyPrice : plan.YearlyPrice;
            var addOnPrice = addOns.Sum(a => cycle == BillingCycle.Monthly ? a.MonthlyPrice : a.YearlyPrice);
            
            return (planPrice + addOnPrice) * userCount;
        }
    }
}
