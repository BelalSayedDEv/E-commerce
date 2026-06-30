using E_Commerce.Contracts;
using E_Commerce.DTos.Order;
using E_Commerce.DTos.ProductDTOs;
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

            switch (result.Outcome)
            {
                case OrderOutcome.ProductDeleted:
                    return NotFound(ApiResponse<object>.Failure(result.Message!));

                case OrderOutcome.NotEnoughStock:
                    return Conflict(ApiResponse<object>.Failure(result.Message!));

                case OrderOutcome.CartItemsEmpty:
                    return Conflict(ApiResponse<object>.Failure(result.Message!));

                case OrderOutcome.Error:
                    return BadRequest(ApiResponse<object>.Failure(result.Message!));

            }
            return Created("", ApiResponse<OrderDto>.Success(result.OrderDto));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersResult()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var OrderHistory = await orderService.GetOrdersHistory(UserId);

            return Ok(ApiResponse<List<OrderDto>>.Success(OrderHistory));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/orders")]
        public async Task<IActionResult> GetOrdersHistoryForAdmin()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var OrderHistory = await orderService.GetOrdersHistoryForAdmin();

            return Ok(ApiResponse<List<OrderDto>>.Success(OrderHistory));
        }

        [HttpPatch("{Id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int Id, UpdateOrderStatus dto)
        {
            var result = await orderService.UpdateStatus(Id, dto.Status);

            switch (result.Outcome)
            {

                case OrderOutcome.SameStatus:
                    return Conflict(ApiResponse<object>.Failure(result.Message!));
                case OrderOutcome.OrderNotFound:
                    return NotFound(ApiResponse<object>.Failure(result.Message!));
            }

            return Ok(ApiResponse<OrderDto>.Success(result.OrderDto));
        }


    }
}
