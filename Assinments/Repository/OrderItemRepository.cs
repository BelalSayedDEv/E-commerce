using Assinments.Model;

namespace Assinments.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ApplicationDbContext context;

        public OrderItemRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(OrderItem orderItem)
        {
            context.Add(orderItem);
        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
