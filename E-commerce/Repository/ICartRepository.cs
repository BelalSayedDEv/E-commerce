using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface ICartRepository
    {
        public Task AddCart(Cart cart);
        public Task AddItemToCart(CartItem cartItem);
        public Task<CartItem?> GetCartItemById(int CartItem);

        public Task<CartItem?> GetCartItemByProductIdAndCartId(int ProductId, int CartId);
        public void RemoveItemFromCart(CartItem cartItem);
        public Task<Cart?> GetCartByUserId(string UserId);
        public Task<List<CartItem>> GetCartItems(int CartId);
        public Task Save();

    }
}
