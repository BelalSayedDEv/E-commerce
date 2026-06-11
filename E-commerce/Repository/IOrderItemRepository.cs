using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface IOrderItemRepository
    {
        public void Add(OrderItem orderItem);

        public void Save();
    }
}
