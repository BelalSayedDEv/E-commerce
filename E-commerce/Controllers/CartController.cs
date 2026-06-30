using E_Commerce.Contracts;
using E_Commerce.DTos.Cart;
using E_Commerce.Model;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }


        [HttpGet]
        public async Task<IActionResult> GetCart()
        {

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            CartDto cartDto = await cartService.GetCartAsync(id);

            return Ok(ApiResponse<CartDto>.Success(cartDto));

        }


        [HttpPost]
        public async Task<IActionResult> AddCartItem(AddToCartDto dto)
        {

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await cartService.AddToCart(id, dto);

            switch (result.Outcome)
            {
                case CartOutcome.ProductNotFound:
                    return NotFound(ApiResponse<object>.Failure(result.Message!));

                case CartOutcome.QuantityUpdated:
                    return Ok(ApiResponse<CartItemDto>.Success(result.Item, "Quantity updated"));

                case CartOutcome.NotEnoughStock:
                    return BadRequest(ApiResponse<object>.Failure(result.Message!));
            }

            return Created("", ApiResponse<CartItemDto>.Success(result.Item));
        }


        [HttpDelete("{ItemId}")]
        public async Task<IActionResult> DeleteFromCart(int ItemId)
        {

            var result = await cartService.RemoveFromCart(ItemId);

            if (!result)
                return NotFound(ApiResponse<object>.Failure("CartItem not found"));

            return NoContent();
        }

    }

}
