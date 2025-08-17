using MyShop_Site.Models;

namespace ShopSite_Master.Services.Master
{
    public class OrderService
    {

        public OrderService()
        {}

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.OrderNumber = GenerateOrderNumber();
            order.CreatedDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            // request to add order 
    

            return order;
        }

        //public async Task<Order?> GetOrderAsync(int orderId)
        //{
        //   //request to get user orders 
        //}

        //public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        //{
        //    // request GetOrderByNumberAsync
        //}

        //public async Task<Order> CompleteOrderAsync(int orderId)
        //{
       // 1- get by id 
        // 
        //    return order;
        //}

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
