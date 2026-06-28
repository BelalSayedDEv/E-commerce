using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface IOrderRepository
    {
        public Order Add(Order order);

        public List<Order> GetOrders();
        public List<Order> GetOrders(string userId);

        public Order? GetOrderById(int Id);
        public void Save();

        public ApplicationDbContext Context();
    }
}
