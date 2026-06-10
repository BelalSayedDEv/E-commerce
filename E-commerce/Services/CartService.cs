using E_commerce.DTos.Cart;
using E_commerce.Model;
using E_commerce.Repository;

namespace E_commerce.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository cartRepository;
        private readonly ICartItemRepository cartItemRepository;
        private readonly IproductService productservice;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IproductService productservice)
        {
            this.cartRepository = cartRepository;
            this.cartItemRepository = cartItemRepository;
            this.productservice = productservice;
        }

        public void AddToCart(string userId, AddToCartDto dto)
        {

            var cart = cartRepository.GetCartByUserId(userId);


            if (cart == null)
            {
                cart = new Cart { UserID = userId };
                cartRepository.AddCart(cart);

            }

            CartItem cartItem = new CartItem();

            cartItem.ProductID = dto.ProductId;
            cartItem.Quantity = dto.Quantity;
            cartItem.CartId = cart.Id;

            cartItemRepository.AddCartItem(cartItem);
            cartItemRepository.Save();

        }

        public CartDto? GetCart(string userId)
        {
            var Cart = cartRepository.GetCartByUserId(userId);

            if (Cart == null)
                return null;

            var CartItem = cartItemRepository.GetCartItemsByCartId(Cart.Id).ToList();

            List<CartItemDto> cartItems = new List<CartItemDto>();

            double TotalPrice = 0;
            foreach (var item in CartItem)
            {
                CartItemDto cartItem = new CartItemDto();
                var product = productservice.GetProductById(item.ProductID);

                cartItem.ProductName = product.Name;
                cartItem.Price = product.Price;
                cartItem.Quantity = item.Quantity;
                cartItem.SubTotal = cartItem.Price * cartItem.Quantity;
                cartItem.Id = item.Id;
                TotalPrice += cartItem.SubTotal;

                cartItems.Add(cartItem);
            }



            CartDto cartDto = new CartDto();

            cartDto.Products = cartItems;
            cartDto.TotalPrice = TotalPrice;

            return cartDto;
        }

        public bool RemoveFromCart(int cartItemId)
        {
            var CartItem = cartItemRepository.RemoveCartItem(cartItemId);
            if (!CartItem)
                return false;

            cartItemRepository.Save();
            return true;

        }
    }
}
