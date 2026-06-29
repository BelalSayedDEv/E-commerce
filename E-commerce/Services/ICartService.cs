using E_Commerce.Contracts;
using E_Commerce.DTos.Cart;

namespace E_Commerce.Services
{
    public interface ICartService
    {
        public Task<CartDto> GetCartAsync(string userId);
        public Task<CartResult> AddToCart(string userId, AddToCartDto dto);
        public Task<bool> RemoveFromCart(int cartItemId);
    }
}
