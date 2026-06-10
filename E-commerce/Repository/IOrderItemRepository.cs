using E_commerce.Model;

namespace E_commerce.Repository
{
    public interface IOrderItemRepository
    {
        public void Add(OrderItem orderItem);

        public void Save();
    }
}
