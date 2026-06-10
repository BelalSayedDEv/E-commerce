using E_commerce.Model;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Repository
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

        public List<Order> GetOrders(string UserId)
        {
            var Orders = context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == UserId)
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
    }
}
