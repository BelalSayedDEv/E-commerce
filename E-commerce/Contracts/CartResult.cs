using E_Commerce.DTos.Cart;

namespace E_Commerce.Contracts
{
    public class CartResult
    {
        public CartOutcome Outcome { get; set; }

        public string? Message { get; set; }

        public CartItemDto? Item { get; set; }

        public int? AvailableStock { get; set; }
    }
}
