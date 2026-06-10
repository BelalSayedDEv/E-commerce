using Assinments.Model;

namespace Assinments.Repository
{
    public interface IOrderRepository
    {
        public Order Add(Order order);

        public List<Order> GetOrders(string UserId);

        public Order? GetOrderById(int Id);
        public void Save();
    }
}
