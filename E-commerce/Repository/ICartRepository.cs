using Assinments.Model;

namespace Assinments.Repository
{
    public interface ICartRepository
    {

        public Cart GetCartByUserId(string UserId);

        public void Save();

        public void AddCart(Cart cart);
    }
}
