using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface IOrderRepository
    {
        public Task AddNewOrderItem(OrderItem orderItem);
        public Task AddNewOrder(Order order);
        public Task<List<Order>> GetOrders();
        public Task<List<Order>> GetOrdersByUserId(string userId);
        public Task<Order?> GetOrderById(int Id);
        public Task Save();
        public ApplicationDbContext Context();
    }
}
