using E_Commerce.DTos.Order;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await orderService.MakeOrder(userId);

            if (result == null)
                return BadRequest(ApiResponse<Object>.Failure("Cart is empty or stock insufficient"));

            return Ok(ApiResponse<OrderDto>.Success(result));
        }

        [HttpGet("History")]
        public IActionResult GetOrdersResult()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var OrderHistory = orderService.GetOrdersHistory(UserId);

            if (OrderHistory == null)
                return NotFound(ApiResponse<object>.Failure("No Orders Found"));

            return Ok(ApiResponse<List<OrderDto>>.Success(OrderHistory));
        }

        [HttpGet("admin/History")]
        public IActionResult GetOrdersHistoryForAdmin()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var OrderHistory = orderService.GetOrdersHistoryForAdmin();

            if (OrderHistory == null)
                return NotFound(ApiResponse<object>.Failure("No Orders Found"));

            return Ok(ApiResponse<List<OrderDto>>.Success(OrderHistory));
        }



        [HttpPatch("{Id}/Status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int Id, string Status)
        {
            var order = orderService.UpdateStatus(Id, Status);
            if (order == null)
                return NotFound(ApiResponse<object>.Failure("Order Not Found"));

            return Ok(ApiResponse<OrderDto>.Success(order));
        }


    }
}
