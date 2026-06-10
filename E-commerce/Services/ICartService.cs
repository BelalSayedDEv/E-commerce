using E_commerce.DTos.Cart;

namespace E_commerce.Services
{
    public interface ICartService
    {
        public void AddToCart(string userId, AddToCartDto dto);
        public CartDto? GetCart(string userId);
        public bool RemoveFromCart(int cartItemId);
    }
}
