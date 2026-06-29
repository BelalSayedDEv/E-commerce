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

        public async Task AddNewOrderItem(OrderItem orderItem)
        {
            await context.AddAsync(orderItem);
        }
        public async Task AddNewOrder(Order order)
        {
            await context.Orders.AddAsync(order);
        }

        public async Task<List<Order>> GetOrders()
        {
            var Orders = await context.Orders.AsNoTracking()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return Orders;
        }
        public async Task<List<Order>> GetOrdersByUserId(string userId)
        {
            var Orders = await context.Orders.AsNoTracking()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return Orders;
        }
        public async Task<Order?> GetOrderById(int Id)
        {
            return await context.Orders
                    .Include(o => o.Items)
                     .ThenInclude(i => i.Product)
                     .SingleOrDefaultAsync(o => o.Id == Id);

        }
        public async Task Save()
        {
            await context.SaveChangesAsync();
        }

        public ApplicationDbContext Context()
        {
            return this.context;
        }
    }
}
