using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public class CartItemRepository : ICartItemRepository

    {
        private readonly ApplicationDbContext context;

        public CartItemRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void AddCartItem(CartItem cartItem)
        {
            context.Add(cartItem);
        }

        public List<CartItem> GetCartItemsByCartId(int Id)
        {
            var CartItems = context.CartItems.Where(c => c.CartId == Id).ToList();

            return CartItems;
        }

        public bool RemoveCartItem(int cartItemId)
        {
            var cartItem = context.CartItems.FirstOrDefault(c => c.Id == cartItemId);
            if (cartItem != null)
            {
                context.Remove(cartItem);
                return true;
            }
            return false;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
