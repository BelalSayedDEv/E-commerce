using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface ICartItemRepository
    {
        public List<CartItem> GetCartItemsByCartId(int Id);
        public void AddCartItem(CartItem cartItem);

        public bool RemoveCartItem(int cartItemId);

        public void Save();
    }
}
