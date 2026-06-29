using E_Commerce.Contracts;
using E_Commerce.DTos.Order;

namespace E_Commerce.Services
{
    public interface IOrderService
    {
        Task<OrderResult> MakeOrder(string userId);
        public Task<List<OrderDto>> GetOrdersHistoryForAdmin();
        public Task<List<OrderDto>> GetOrdersHistory(string userId);
        public Task<OrderResult> UpdateStatus(int Id, string status);
    }
}
