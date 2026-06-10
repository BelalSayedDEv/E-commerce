using E_commerce.Model;

namespace E_commerce.Repository
{
    public interface ICartItemRepository
    {
        public List<CartItem> GetCartItemsByCartId(int Id);
        public void AddCartItem(CartItem cartItem);

        public bool RemoveCartItem(int cartItemId);

        public void Save();
    }
}
