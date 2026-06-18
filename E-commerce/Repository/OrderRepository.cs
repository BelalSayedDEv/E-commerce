using E_Commerce.Model;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Order Add(Order order)
        {
            context.Orders.Add(order);
            return order;
        }

        public List<Order> GetOrders()
        {
            var Orders = context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return Orders;
        }

        public Order? GetOrderById(int Id)
        {
            return context.Orders
                    .Include(o => o.Items)
                     .ThenInclude(i => i.Product)
                     .SingleOrDefault(o => o.Id == Id);

        }
        public void Save()
        {
            context.SaveChanges();
        }

        public ApplicationDbContext Context()
        {
            return this.context;
        }
    }
}
