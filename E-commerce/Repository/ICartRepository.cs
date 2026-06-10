using E_commerce.Model;

namespace E_commerce.Repository
{
    public interface ICartRepository
    {

        public Cart? GetCartByUserId(string UserId);

        public void Save();

        public void AddCart(Cart cart);
    }
}
