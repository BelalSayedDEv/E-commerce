using E_Commerce.Contracts;
using E_Commerce.DTos.Cart;
using E_Commerce.Model;
using E_Commerce.Repository;

namespace E_Commerce.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository cartRepository;
        private readonly IProductRepository productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
        }



        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await cartRepository.GetCartByUserId(userId);

            if (cart == null)
            {
                var NewCart = new Cart()
                {
                    UserID = userId,
                    TotalPrice = 0,
                };
                await cartRepository.AddCart(NewCart);
                await cartRepository.Save();

                return new CartDto()
                {
                    Products = new List<CartItemDto>(),
                    TotalPrice = 0
                };
            }

            var Items = await cartRepository.GetCartItems(cart.Id);

            List<CartItemDto> ItemsDto = new List<CartItemDto>();

            foreach (var item in Items)
            {

                CartItemDto cartItemDto = new CartItemDto()
                {
                    Id = item.Id,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Quantity,
                    SubTotal = item.Quantity * item.Product.Price,
                };
                ItemsDto.Add(cartItemDto);

            }
            var CartDto = new CartDto()
            {
                Products = ItemsDto,
                TotalPrice = ItemsDto.Sum(i => i.SubTotal)
            };

            return CartDto;
        }




        public async Task<CartResult> AddToCart(string userId, AddToCartDto dto)
        {
            var cart = await cartRepository.GetCartByUserId(userId);

            if (cart == null)
            {
                var NewCart = new Cart()
                {
                    UserID = userId,
                    TotalPrice = 0,
                };
                await cartRepository.AddCart(NewCart);
                await cartRepository.Save();
                cart = NewCart;
            }

            var product = await productRepository.GetProductByIdAsync(dto.ProductId);

            if (product == null)
                return new CartResult
                {
                    Outcome = CartOutcome.ProductNotFound,
                    Message = "Product not found"
                };

            var Item = await cartRepository.GetCartItemByProductIdAndCartId(dto.ProductId, cart.Id);

            if (Item is null)
            {
                if (dto.Quantity <= product.Quantity)
                {
                    CartItem cartItem = new CartItem()
                    {
                        Quantity = dto.Quantity,
                        ProductID = product.Id,
                        Cart = cart
                    };
                    await cartRepository.AddItemToCart(cartItem);
                    await cartRepository.Save();

                    return new CartResult
                    {
                        Outcome = CartOutcome.ItemAdded,
                        Item = new CartItemDto()
                        {
                            Id = cartItem.Id,
                            Quantity = dto.Quantity,
                            ProductName = product.Name,
                            Price = product.Price,
                            SubTotal = product.Price * dto.Quantity,

                        }

                    };
                }
                return new CartResult
                {
                    Outcome = CartOutcome.NotEnoughStock,
                    AvailableStock = product.Quantity,
                    Message = "The Total Stock is not Enough",
                };
            }

            if ((dto.Quantity + Item.Quantity) <= product.Quantity)
            {
                Item.Quantity += dto.Quantity;
                await cartRepository.Save();
                return new CartResult
                {
                    Outcome = CartOutcome.QuantityUpdated,
                    Item = new CartItemDto()
                    {
                        Id = Item.Id,
                        Quantity = Item.Quantity,
                        ProductName = product.Name,
                        Price = product.Price,
                        SubTotal = product.Price * Item.Quantity,

                    }
                };
            }
            else
            {
                return new CartResult
                {
                    Outcome = CartOutcome.NotEnoughStock,
                    AvailableStock = product.Quantity - Item.Quantity,
                    Message = "The Total Stock is not Enough",
                };
            }
        }

        public async Task<bool> RemoveFromCart(int cartItemId)
        {
            var item = await cartRepository.GetCartItemById(cartItemId);
            if (item != null)
            {
                cartRepository.RemoveItemFromCart(item);
                await cartRepository.Save();
                return true;
            }
            return false; // NotFound 
        }
    }
}
