using E_Commerce.DTos.Order;

namespace E_Commerce.Contracts
{
    public class OrderResult
    {
        public OrderOutcome Outcome { get; set; }
        public string? Message { get; set; }
        public string? ProductName { get; set; }
        public OrderDto? OrderDto { get; set; }
        public int? AvailableStock { get; set; }


    }
}
