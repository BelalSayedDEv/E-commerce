using E_commerce.DTos.Cart;
using E_commerce.Model;
using E_commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.Controllers
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

        [HttpPost]
        public IActionResult AddCartItem(AddToCartDto dto)
        {
            var claimsList = User.Claims.Select(c => $"{c.Type} = {c.Value}").ToList();

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            Console.WriteLine($"NameIdentifier claim: '{(id ?? "NULL")}'");

            cartService.AddToCart(id, dto);

            return Ok(ApiResponse<object>.Success(null, "Item added in Card"));

        }
        [HttpGet]
        public IActionResult GetCart()
        {

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            CartDto? cartDto = cartService.GetCart(id);

            if (cartDto == null)
                return NotFound(ApiResponse<object>.Failure("Not Found"));


            return Ok(ApiResponse<CartDto>.Success(cartDto));

        }

        [HttpDelete("{ItemId}")]
        public IActionResult DeleteCart(int ItemId)
        {

            var result = cartService.RemoveFromCart(ItemId);
            if (!result)
                return NotFound(ApiResponse<object>.Failure("Not Found"));

            return Ok(ApiResponse<Object>.Success(null, "Deleted Successfuly"));
        }

    }

}
