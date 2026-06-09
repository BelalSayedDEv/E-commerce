using Assinments.Model;

namespace Assinments.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext context;

        public CartRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void AddCart(Cart cart)
        {
            context.Add(cart);
        }

        public Cart GetCartByUserId(string UserId)
        {
            var cart = context.Carts.FirstOrDefault(c => c.UserID == UserId);

            return cart;
        }

        public void Save()
        {
            context.SaveChanges();
        }


    }
}
