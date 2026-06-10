using E_commerce.DTos.Order;

namespace E_commerce.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> MakeOrder(string userId);

        public List<OrderDto>? GetOrdersHistory(string userId);

        public OrderDto? UpdateStatus(int Id, string status);
    }
}
