using Assinments.Model;

namespace Assinments.Repository
{
    public interface IOrderItemRepository
    {
        public void Add(OrderItem orderItem);

        public void Save();
    }
}
