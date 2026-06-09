using Assinments.DTos.Cart;
using Assinments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Assinments.Controllers
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

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine($"NameIdentifier claim: '{(id ?? "NULL")}'");

            cartService.AddToCart(id, dto);

            return NoContent();

        }
        [HttpGet]
        public IActionResult GetCart()
        {

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            CartDto cartDto = cartService.GetCart(id);
            if (cartDto == null)
                return NoContent();


            return Ok(cartDto);

        }

        [HttpDelete("{ItemId}")]
        public IActionResult DeleteCart(int ItemId)
        {

            var result = cartService.RemoveFromCart(ItemId);
            if (!result)
                return NotFound();

            return NoContent();
        }

    }

}
