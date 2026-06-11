using E_Commerce.DTos.Cart;

namespace E_Commerce.Services
{
    public interface ICartService
    {
        public void AddToCart(string userId, AddToCartDto dto);
        public Task<CartDto?> GetCartAsync(string userId);
        public bool RemoveFromCart(int cartItemId);
    }
}
