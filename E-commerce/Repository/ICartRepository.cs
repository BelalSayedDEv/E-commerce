using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface ICartRepository
    {

        public Cart? GetCartByUserId(string UserId);

        public void Save();

        public void AddCart(Cart cart);
    }
}
