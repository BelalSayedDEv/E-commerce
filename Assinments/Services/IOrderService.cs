using Assinments.DTos.Order;

namespace Assinments.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> MakeOrder(string userId);

        public List<OrderDto> GetOrdersHistory(string userId);

        public OrderDto UpdateStatus(int Id, string status);
    }
}
