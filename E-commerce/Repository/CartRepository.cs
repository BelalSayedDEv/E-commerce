using E_Commerce.Model;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext context;

        public CartRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task AddCart(Cart cart)
        {
            await context.Carts.AddAsync(cart);
        }

        public async Task AddItemToCart(CartItem cartItem)
        {
            await context.CartItems.AddAsync(cartItem);
        }

        public async Task<Cart?> GetCartByUserId(string UserId)
        {
            var Cart = await context.Carts.FirstOrDefaultAsync(c => c.UserID == UserId);
            return Cart;
        }

        public async Task<List<CartItem>> GetCartItems(int CartId)
        {
            var Items = await context.CartItems.Include(c => c.Product).Where(c => c.CartId == CartId).ToListAsync();
            return Items;
        }

        public async Task<CartItem?> GetCartItemById(int CartItem)
        {
            var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == CartItem);
            return item;
        }


        public void RemoveItemFromCart(CartItem cartItem)
        {
            context.CartItems.Remove(cartItem);
        }
        public async Task<CartItem?> GetCartItemByProductIdAndCartId(int ProductId, int CartId)
        {
            var item = await context.CartItems.FirstOrDefaultAsync(c => c.ProductID == ProductId && c.CartId == CartId);
            return item;
        }
        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
